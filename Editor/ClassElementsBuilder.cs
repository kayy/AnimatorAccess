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
				className = existingClassBuilder.ClassName;
				config = ConfigFactory.Get (className);
				generator = new SmartFormatTemplateEngine ();
				builder = new AnimatorCodeElementsBuilder (go, className, config);
			} else {
				Logger.Error ("Cannot access component AnimatorAccess.BaseAnimatorAccess from object " + go.name);
			}
		}

		/// <summary>
		/// Builds the new and the existing classes' ClassCodeElements, evaluates all changes and returns a list of them.
		/// </summary>
		/// <param name="go">Go.</param>
		public List<ClassMemberCompareElement> Compare (GameObject go) {
			CodeGeneratorResult result = BuildClasses ();
			if (result.Success) {
				if (!existingClass.IsEmpty ()) {
					List<ClassMemberCompareElement> comparisonResult = CodeElementUtils.CompareClasses (existingClass, newClass, 
						config.IgnoreExistingCode, config.KeepObsoleteMembers);
					comparisonResult.RemoveAll ((element) => element.Member == "Awake");
					string message = "";
					comparisonResult.ForEach ((s) => message += s + "\n");
					Logger.Debug ("Comparison between new and existing class reveals " + comparisonResult.Count + " changes: " + message);
					return comparisonResult;
				}
			}
			return new List<ClassMemberCompareElement> ();
		}

		CodeGeneratorResult BuildClasses () {
			TemplateLookup templateLookup = new TemplateLookup (config);
			CodeGeneratorResult result = templateLookup.GetPathToTemplate (className);
			if (result.Success) {
				result = generator.Prepare (templateLookup.TemplateConfig);
				if (result.NoSuccess) {
					return result;
				}
				newClass = builder.Build ();
				if (newClass.IsEmpty ()) {
					return result.SetError ("No Input", "The input seems to be invalid. Check that there are any states or parameter to process.");
				}
				Logger.Debug ("New: " + newClass);
				if (!existingClassBuilder.HasType ()) {
					Logger.Info ("Generating source for " + className + " the very first time");
				}
				try {
					existingClassBuilder.MethodInfoFilter = (MethodInfo mi) => mi.Name.StartsWith ("Is") || 
						mi.Name.StartsWith ("Set") || mi.Name.StartsWith ("Get") || mi.Name == "StateIdToName";
					existingClass = existingClassBuilder.Build ();
				} catch (System.Exception ex) {
					Logger.Warning (ex.Message + "\n" + ex.StackTrace);
					result.SetError ("Error", "Oops. An unexpected error occurred. Details" + ex.Message + "\n" + ex.StackTrace);
				}
			}
			return result;
		}

		public CodeGeneratorResult PrepareCodeGeneration (bool forceUpdate) {
			CodeGeneratorResult result = BuildClasses ();
			if (result.Error) {
				return result;
			}
			if (!existingClass.IsEmpty () && !config.IgnoreExistingCode) {
				int remaining = CodeElementUtils.CleanupExistingClass (existingClass, newClass, config.KeepObsoleteMembers);
				if (remaining > 0 && !forceUpdate) {
					string consoleMessage = "";
					List<MemberCodeElement> previousMembers = existingClass.GetAllMembers ();
					int previousMembersCount = previousMembers.Count;
					for (int i = 0; i < previousMembersCount; i++) {
						consoleMessage += previousMembers [i].GetSignature () + "\n";
					}
					Logger.Info ("Members found in previous version that will be marked as obsolete: " + consoleMessage);
				}
			}
			return result;
		}
	
		public CodeGeneratorResult GenerateCode () {
			if (!config.IgnoreExistingCode) {
				if (existingClass != null  && !existingClass.IsEmpty ()) {
					string msg = string.Format ("Animator state or parameter is no longer valid{0}. Refactor your code to not contain any references.", (config.KeepObsoleteMembers ? "" : " and will be removed in the next code generation"));
					existingClass.AddAttributeToAllMembers (new ObsoleteAttributeCodeElement (msg, false));
					List<MemberCodeElement> allMembers = newClass.GetAllMembers ();
					newClass.MergeMethods (existingClass, (element) => !allMembers.Contains (element));
					newClass.MergeProperties (existingClass,(element) => !allMembers.Contains (element));
					newClass.MergeFields (existingClass, (element) => !allMembers.Contains (element));
				}
			}
			FileCodeElement fileElement = new FileCodeElement (newClass);
			fileElement.Usings.Add (new UsingCodeElement ("UnityEngine"));
			fileElement.Usings.Add (new UsingCodeElement ("System.Collections"));
			CodeGeneratorResult result = generator.GenerateCode (fileElement);
			if (result.Success) {
				code = generator.Code;
			}
			return result;
		}

	}
}
