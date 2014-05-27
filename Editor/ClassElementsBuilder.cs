// The MIT License (MIT)
// 
// Copyright (c) 2014 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Scio.CodeGeneration;


namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// Code generator to create a class for convenient access to Animator states and parameters. 
	/// The generated class contains methods to query Animator states like: 
	/// 	public bool Is<AnimatorStatePrefix><LayerName><AnimationState> (int nameHash)
	/// Parameter access is done by properties:
	///     public float <ParameterPrefix><ParameterName>
	/// AnimatorStatePrefix and ParameterPrefix are empty by default. The layer name is used
	/// for all layers higher than 0 by default (can be forced for layer 0 by option forceLayerPrefix).
	/// Example of generated code:
	/// 	public bool IsIdle (int nameHash) { // <= Base.Idle
	///         return nameHash == -1405706489;
	///     }
	///     public float Speed  { // <= Speed ,default: 1
	///         get { return animator.GetFloat (-823668238); }
	///         set { animator.SetFloat (-823668238, value); }
	///     }
	/// </summary>
	public class ClassElementsBuilder
	{
		/// <summary>
		/// Caches the last template directory location in order to prevent full directory scan on subsequent calls.
		/// </summary>
		public static string LastTemplateDirectoryCache = "";
		
		AnimatorCodeElementsBuilder builder;
		ClassCodeElement newClass = null;
	
		ReflectionCodeElementsBuilder existingClassBuilder;
		ClassCodeElement existingClass = null;
	
		TemplateEngine generator;
		Config config;
	
		string className = "";

		public string ClassName {
			get { return className; }
		}
		public string FullClassName {
			get { return (config.DefaultNamespace == null ? "" : config.DefaultNamespace + ".") + className; }
		}

		string pathToTemplate = "";
	
		string code = "";
		public string Code { get { return code; } }
	
		/// <summary>
		/// Contructor in case of the very first generation of an AnimatorAccess class. On subsequent generations 
		/// ClassElementsBuilder (GameObject go) is called.
		/// </summary>
		/// <param name="go">GameObject to generate an AnimatorAccess class for.</param>
		/// <param name="fileName">File name where to save the file. A subdirectory 'Generated' is recommended to 
		/// emphasize that this code shouldn't be edited.</param>
		public ClassElementsBuilder (GameObject go, string fileName)
		{
			className = Path.GetFileNameWithoutExtension (fileName);
			config = ConfigFactory.Get (className);
			generator = new SmartFormatTemplateEngine ();
			builder = new AnimatorCodeElementsBuilder (go, className, config);
			existingClassBuilder = new ReflectionCodeElementsBuilder ("Assembly-CSharp", config.DefaultNamespace, className);
		}
	
		/// <summary>
		/// Udpate contructor.
		/// </summary>
		/// <param name="go">GameObject to generate an AnimatorAccess class for.</param>
		public ClassElementsBuilder (GameObject go)
		{
			AnimatorAccess.BaseAnimatorAccess animatorAccess = go.GetComponent<AnimatorAccess.BaseAnimatorAccess> ();
			if (animatorAccess != null) {
				existingClassBuilder = new ReflectionCodeElementsBuilder (animatorAccess);
			}
			className = existingClassBuilder.ClassName;
			config = ConfigFactory.Get (className);
			generator = new SmartFormatTemplateEngine ();
			builder = new AnimatorCodeElementsBuilder (go, className, config);
		}

		/// <summary>
		/// Builds the new and the existing classes' ClassCodeElements, evaluates all changes and returns a list of them.
		/// </summary>
		/// <param name="go">Go.</param>
		public List<ClassMemberCompareElement> Compare (GameObject go) {
			CodeGeneratorResult result = BuildClasses ();
			if (result.Success) {
				if (!existingClass.IsEmpty ()) {
					List<ClassMemberCompareElement> comparisonResult = CodeElementUtils.CompareClasses (existingClass, newClass, config.KeepObsoleteMembers);
					comparisonResult.RemoveAll ((element) => element.member == "Awake");
					string message = "";
					comparisonResult.ForEach ((s) => message += s + "\n");
					return comparisonResult;
				}
			}
			return new List<ClassMemberCompareElement> ();
		}

		CodeGeneratorResult BuildClasses () {
			CodeGeneratorResult result = GetPathToTemplate ();
			if (result.Success) {
				TemplateEngineConfig codeGeneratorConfig = new TemplateEngineConfig (pathToTemplate);
				result = generator.Prepare (codeGeneratorConfig);
				if (result.NoSuccess) {
					return result;
				}
				newClass = builder.Build ();
				if (newClass.IsEmpty ()) {
					return result.SetError ("No Input", "The input seems to be invalid. Check that there are any states or parameter to process.");
				}
				Logger.Debug ("New: " + newClass);
				if (existingClassBuilder.HasType ()) {
					try {
						existingClassBuilder.MethodInfoFilter = (MethodInfo mi) => mi.Name.StartsWith ("Is");
						existingClass = existingClassBuilder.Build ();
					} catch (System.Exception ex) {
						Debug.LogWarning (ex.Message + "\n" + ex.StackTrace);
						result.SetError ("Error", "Oops. An unexpected error occurred. Details" + ex.Message + "\n" + ex.StackTrace);
					}
				} else {
					Logger.Info ("Generating source for " + className + " the very first time");
				}
			}
			return result;
		}

		public CodeGeneratorResult PrepareCodeGeneration (bool forceUpdate) {
			CodeGeneratorResult result = BuildClasses ();
			if (result.Error) {
				return result;
			}
			if (!existingClass.IsEmpty ()) {
				int remaining = CodeElementUtils.CleanupExistingClass (existingClass, newClass, config.KeepObsoleteMembers);
				if (remaining > 0 && !forceUpdate) {
					string removedMembers = "";
					string consoleMessage = "";
					List<string> previousMembers = CodeElementUtils.GetCriticalNames (existingClass);
					for (int i = 0; i < remaining; i++) {
						consoleMessage += previousMembers [i] + "\n";
						if (i < 3) {
							removedMembers += previousMembers [i] + "\n";
						} else
						if (i >= remaining - 1) {
							removedMembers += "... (" + (remaining - 3) + " more)\n";
						}
					}
					Logger.Debug ("Members found in previous version that disappeared now: " + consoleMessage);
					string s = string.Format ("The following members are found in the previous version of {0} but will not be " + "created again:\n{1}\n(See console for details)\nClick 'OK' to generate new version. Click 'Cancel' if you want" + " to refactor your code first if other classes refer to these members.", className, removedMembers);
					result.SetWarning (remaining + " Removed Members", s);
				}
			}
			return result;
		}
	
		public CodeGeneratorResult GenerateCode () {
			if (!config.ForceOverwritingOldClass) {
				if (existingClass != null  && !existingClass.IsEmpty ()) {
					string msg = string.Format ("Animator state or parameter is no longer valid{0}. Refactor your code to not contain any references.", (config.KeepObsoleteMembers ? "" : " and will be removed in the next code generation"));
					existingClass.AddAttributeToAllMembers (new ObsoleteAttributeCodeElement (msg, false));
					newClass.MergeMethods (existingClass);
					newClass.MergeProperties (existingClass);
				}
			}
			FileCodeElement fileElement = new FileCodeElement (newClass);
			fileElement.Usings.Add (new UsingCodeElement ("UnityEngine"));
			CodeGeneratorResult result = generator.GenerateCode (fileElement);
			if (result.Success) {
				code = generator.Code;
			}
			return result;
		}
	
		public CodeGeneratorResult GetPathToTemplate ()
		{
			CodeGeneratorResult result = new CodeGeneratorResult ();
			LastTemplateDirectoryCache = Preferences.GetString (Preferences.Key.TemplateDir);
			if (string.IsNullOrEmpty (LastTemplateDirectoryCache) || !Directory.Exists (LastTemplateDirectoryCache)) {
				result = SearchTemplateDirectory (result);
				if (result.NoSuccess) {
					return result;
				}
			} else {
				string classSpecificTemplate = Path.Combine (LastTemplateDirectoryCache, className + ".txt");
				if (File.Exists (classSpecificTemplate)) {
					pathToTemplate = classSpecificTemplate;
					return result;
				} else {
					string defaultTemplate = Path.Combine (LastTemplateDirectoryCache, config.GetDefaultTemplateFileName ());
					if (File.Exists (defaultTemplate)) {
						pathToTemplate = defaultTemplate;
						return result;
					} else {
						result = SearchTemplateDirectory (result);
						if (result.NoSuccess) {
							return result;
						}
					}
				}
			}
			string defaultTemplate2 = Path.Combine (LastTemplateDirectoryCache, config.GetDefaultTemplateFileName ());
			if (!File.Exists (defaultTemplate2)) {
				return result.SetError ("Default Template Not Found", "The default template file " + config.GetDefaultTemplateFileName () + " could not be found. Path: " + defaultTemplate2);
			}
			pathToTemplate = defaultTemplate2;
			return result;
		}
	
		CodeGeneratorResult SearchTemplateDirectory (CodeGeneratorResult result)
		{
			LastTemplateDirectoryCache = "";
			pathToTemplate = "";
			string searchRoot = Path.Combine (Application.dataPath, Manager.SharedInstance.InstallDir);
			Logger.Debug ("Searching for default template in " + searchRoot);
			string[] files = Directory.GetFiles (searchRoot, config.GetDefaultTemplateFileName (), SearchOption.AllDirectories);
			if (files.Length == 0) {
				// fallback, scan all directories under Assets folder
				files = Directory.GetFiles (Application.dataPath, config.GetDefaultTemplateFileName (), SearchOption.AllDirectories);
			}
			if (files.Length == 0) {
				return result.SetError ("Template Directory Not Found", "The default template " + config.GetDefaultTemplateFileName () + "could not be found anywhere under your Assets directory.");
			} else if (files.Length > 1) {
				Logger.Info ("More than one default template found. Searching the best match");
				string rootDir = config.PathToTemplateDirectory;
				foreach (string item in files) {
					if (item.Contains (rootDir)) {
						pathToTemplate = item;
						break;
					}
				}
				if (string.IsNullOrEmpty (pathToTemplate)) {
					pathToTemplate = files [0];
					Logger.Debug ("More than one default template found but non of them matching the path " + rootDir);
				}
			} else {
				pathToTemplate = files [0];
			}
			LastTemplateDirectoryCache = Path.GetDirectoryName (pathToTemplate);
			Preferences.SetString (Preferences.Key.TemplateDir, LastTemplateDirectoryCache);
			return result;
		}
	}
}
