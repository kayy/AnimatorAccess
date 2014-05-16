// The MIT License (MIT)
// 
// Copyright (c) 2014 kayy
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
using UnityEditorInternal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Scio.CodeGenerator;


/// <summary>
/// Code generator to create a class for convenient access to Animator states and parameters. 
/// The generated class contains methods to query Animator states like: 
/// 	public bool Is<AnimationStatePrefix><LayerName><AnimationState> (int nameHash)
/// Parameter access is done by properties:
///     public float <ParameterPrefix><ParameterName>
/// AnimationStatePrefix and ParameterPrefix are empty by default. The layer name is used
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
public class AnimatorWrapperGenerator
{
	/// <summary>
	/// Caches the last template directory location in order to prevent full directory scan on subsequent calls.
	/// </summary>
	public static string LastTemplateDirectoryCache = "";
	
	AnimatorCodeElementsBuilder builder;
	ClassCodeElement newClass = null;

	ReflectionCodeElementsBuilder existingClassBuilder;
	ClassCodeElement existingClass = null;

	CodeGenerator generator;
	AnimatorWrapperConfig config;

	string className;
	string pathToTemplate = "";

	string code = "";
	public string Code { get { return code; } }

	public AnimatorWrapperGenerator (GameObject go, string fileName)
	{
		LastTemplateDirectoryCache = null;
		className = Path.GetFileNameWithoutExtension (fileName);
		config = AnimatorWrapperConfigFactory.Get (className);
		generator = config.Generator;
		builder = new AnimatorCodeElementsBuilder (go, className, config);
		existingClassBuilder = new ReflectionCodeElementsBuilder ("Assembly-CSharp", className);
	}

	public CodeGeneratorResult Prepare () {
		CodeGeneratorResult result = GetPathToTemplate ();
		if (result.Success) {
			CodeGeneratorConfig codeGeneratorConfig = new CodeGeneratorConfig (pathToTemplate);
			result = generator.Prepare (codeGeneratorConfig);
			if (result.NoSuccess) {
				return result;
			}
			newClass = builder.Build ();
			if (newClass == null) {
				return result.SetError ("No Input", "The input seems to be invalid. Check that there are any states or parameter to process.");
			}
			Debug.Log ("New: " + newClass);
			if (existingClassBuilder.HasType ()) {
				try {
					existingClassBuilder.MethodInfoFilter = (MethodInfo mi) => mi.Name.StartsWith ("Is");
					existingClass = existingClassBuilder.Build ();
					Debug.Log ("Old: " + existingClass);
					int remaining = CodeElementUtils.CleanupExistingClass (existingClass, newClass, config.KeepObsoleteMembers);
					if (remaining > 0) {
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
						Debug.Log ("Members found in previous version that disappeared now: " + consoleMessage);
						string s = string.Format ("The following members are found in the previous version of {0} but will not be " + "created again:\n{1}\n(See console for details)\nClick 'OK' to generate new version. Click 'Cancel' if you want" + " to refactor your code first if other classes refer to these members.", className, removedMembers);
						//						Debug.Log ("Code generation cancelled for class " + className + ". The generated code would have been:\n" + code);
						return result.SetWarning (remaining + " Removed Members", s);
					}
					return result;
				} catch (System.Exception ex) {
					Debug.LogWarning (ex.Message + "\n" + ex.StackTrace);
					result.SetError ("Error", "Oops. An unexpected error occurred. Details" + ex.Message + "\n" + ex.StackTrace);
				}
			}
		}
		return result;
	}

	public CodeGeneratorResult GenerateCode () {
		FileCodeElement fileElement = new FileCodeElement (newClass);
		fileElement.Usings.Add (new UsingCodeElement ("UnityEngine"));
		if (!existingClass.IsEmpty ()) {
			// TODO_kay: check exisitng class
			fileElement.Classes.Add (existingClass);
		}
		CodeGeneratorResult result = generator.GenerateCode (fileElement);
		if (result.Success) {
			code = generator.Code;
		}
		return result;
	}

	public CodeGeneratorResult GetPathToTemplate () {
		CodeGeneratorResult result = new CodeGeneratorResult ();
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
		Log.Temp ("Found template at: " + pathToTemplate + " Cache dir is now " + LastTemplateDirectoryCache);
		return result;
	}

	CodeGeneratorResult SearchTemplateDirectory (CodeGeneratorResult result) {
		LastTemplateDirectoryCache = "";
		pathToTemplate = "";
		string[] files = Directory.GetFiles (Application.dataPath, config.GetDefaultTemplateFileName (), SearchOption.AllDirectories);
		if (files.Length == 0) {
			return result.SetError ("Template Directory Not Found", "The default template " + config.GetDefaultTemplateFileName () + "could not be found anywhere under your Assets directory.");
		} else if (files.Length > 1) {
			Debug.Log ("More than one default template found. Searching the best match");
			string rootDir = config.PathToTemplateDirectory;
			foreach (string item in files) {
				if (item.Contains (rootDir)) {
					pathToTemplate = item;
					break;
				}
			}
			if (string.IsNullOrEmpty (pathToTemplate)) {
				pathToTemplate = files [0];
				Debug.Log ("More than one default template found but non of them matching the path " + rootDir);
			}
		} else {
			pathToTemplate = files [0];
		}
		LastTemplateDirectoryCache = Path.GetDirectoryName (pathToTemplate);
		return result;
	}
}
