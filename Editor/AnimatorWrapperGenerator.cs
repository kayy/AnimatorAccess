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

public static class AnimatorWrapperGenerator
{
	static bool forceLayerPrefix = false;
	static string AnimationStatePrefix = "";
	static string ParameterPrefix = "";

	const string resourcesDir = "Scripts";

	/// <summary>
	/// Default name for Save As dialog.
	/// </summary>
	static string targetClassNameDefault = "AnimatorWrapper";
	static string targetCodeFile = targetClassNameDefault + ".cs";
	static string targetClassName = targetClassNameDefault;

	static string userSelection;

	static Animator animator;
	static List<LayerStatesList> allLayerStates;
	// not yet used, but maybe important for multi layer support in the future
	static List<string> layerNames;
	static List<string> previousMembers;

	static string code = "";

	static CodeGenerationUtils cg = new CodeGenerationUtils ();
	
	[MenuItem("Tools/Generate Animator Wrapper")]
	public static void GenerateAnimatorWrapper () {
		allLayerStates = new List<LayerStatesList> ();
		layerNames = new List<string> ();
		previousMembers = new List<string> ();
		if (!CheckPreconditions ()) {
			return;
		}
		RegisterExistingNames ();
		GenerateClass ();
		if ((previousMembers.Count > 0)) {
			string removedMembers = "";
			string consoleMessage = "";
			int count = previousMembers.Count;
			for (int i = 0; i < count; i++) {
				consoleMessage += previousMembers[i] + "\n";
				if (i < 3) {
					removedMembers += previousMembers[i] + "\n";
				} else if (i >= count - 1) {
					removedMembers += "... (" + (count - 3) + " more)\n";
				}
			}
			Debug.Log ("Members found in previous version that disappeared now: " + consoleMessage);
			string s = string.Format ("The following members are found in the previous version of {0} but will not be " +
			                          "created again:\n{1}\n(See console for details)\nClick 'OK' to generate new version. Click 'Cancel' if you want" +
			                          " to refactor your code first if other classes refer to these members.", targetClassName, removedMembers);
			if (!EditorUtility.DisplayDialog (previousMembers.Count + " Removed Members", s, "OK", "Cancel")) {
				Debug.Log ("Code not written to " + targetCodeFile + ". The generated code would have been:\n" + code);
				return ;
			}
		}
		WriteCodeToFile ();
	}

	static void GenerateClass () {
		string versionInfo = System.DateTime.Now  + ", selected game object was " + userSelection;
		code = cg.GenerateMITHeader (versionInfo);
		code += cg.Code (0, "using UnityEngine");
		code += cg.Code (0, "using System.Collections.Generic", 2);
		code += cg.MakeComment (0, "Provides convenient access to parameters and states of the Animator class." +
		                                ""
		                                );
		code += cg.Line (0, "public class " + targetClassName);
		code += cg.Line (0, "{");
		code += cg.Code (1, "const string VersionInfo = \"" + versionInfo + "\"", 2);
		AddClassCode ();
		code += cg.Line (0, "}");
	}

	static void AddClassCode () {
		string memberDeclaration = cg.Code (1, "Animator animator");
		string methodsCode = "";
		string constructor = cg.Line (1, "public " + targetClassName + " (Animator animator) {");
		constructor += cg.Code (2, "this.animator = animator");
		constructor += cg.Line (1, "}", 2);
		constructor += cg.Line (1, "[System.ObsoleteAttribute (\"Default constructor is provided only for internal use (reflection). Use " + targetClassName + " (Animator animator) instead\", true)]");
		constructor += cg.Line (1, "public " + targetClassName + " () {}", 2);
		// build code for checking animator states e.g. public IsMyState (hash) ...
		int layer = 0;
		foreach (LayerStatesList layerStates in allLayerStates) {
			foreach (string item in layerStates.LayerStates) {
				int nameHash = Animator.StringToHash (item);
				string propName = GenerateStateName (AnimationStatePrefix, item, (layerStates.LayerIndex > 0 ? null : layerStates.LayerName));
				string methodName = "Is" + propName;
				RegisterMember (methodName);
				methodsCode += cg.Line (1, "public bool Is" + propName + " (int nameHash) { // <= " + item);
				methodsCode += cg.Code (2, " return nameHash == " + nameHash);
				methodsCode += cg.Line (1, "}");
			}
			layer++;
		}
		AnimatorController animatorController = AnimatorController.GetEffectiveAnimatorController(animator);
		int countParameters = animatorController.parameterCount;
		if (countParameters > 0) {
			// build code for accessing animator parameters as properties e.g.:
			// public MyParam { get { return animator.GetBool (12345); } set {...}}
			methodsCode += "\n";
			for (int i = 0; i < countParameters; i++) {
				AnimatorControllerParameter parameter = animatorController.GetParameter (i);
				int paramHash = Animator.StringToHash (parameter.name);
				string propName = cg.GeneratePropertyName (ParameterPrefix, parameter.name);
				if (parameter.type == AnimatorControllerParameterType.Bool) {
					methodsCode += cg.Line (1, "public bool " + propName + " { // <= " + parameter.name + " ,default: " + parameter.defaultBool);
					methodsCode += cg.Line (2, "get { return animator.GetBool (" + paramHash + "); }");
					methodsCode += cg.Line (2, "set { animator.SetBool (" + paramHash + ", value); }");
					methodsCode += cg.Line (1, "}");
				} else if (parameter.type == AnimatorControllerParameterType.Float) {
					methodsCode += cg.Line (1, "public float " + propName + " { // <= " + parameter.name + " ,default: " + parameter.defaultFloat);
					methodsCode += cg.Line (2, "get { return animator.GetFloat (" + paramHash + "); }");
					methodsCode += cg.Line (2, "set { animator.SetFloat (" + paramHash + ", value); }");
					methodsCode += cg.Line (1, "}");
				} else if (parameter.type == AnimatorControllerParameterType.Int) {
					methodsCode += cg.Line (1, "public float " + propName + " { // <= " + parameter.name + " ,default: " + parameter.defaultInt);
					methodsCode += cg.Line (2, "get { return animator.GetInteger (" + paramHash + "); }");
					methodsCode += cg.Line (2, "set { animator.Setinteger (" + paramHash + ", value); }");
					methodsCode += cg.Line (1, "}");
				} else if (parameter.type == AnimatorControllerParameterType.Trigger) {
					methodsCode += cg.Line (1, "public bool " + propName + " { // <= " + parameter.name);
					methodsCode += cg.Line (2, "set { animator.SetTrigger (" + paramHash + "); }");
					methodsCode += cg.Line (1, "}");
				}
				RegisterMember (propName);
				methodsCode += cg.Line (0 , "");
			}
		}
		code += memberDeclaration + "\n" + constructor + "\n" + methodsCode;
	}

	static void RegisterMember (string member) {
		if (previousMembers.Contains (member)) {
			previousMembers.Remove (member);
		}
	}

	static bool CheckPreconditions () {
		if ((Selection.gameObjects == null) || (Selection.gameObjects.Length != 1)) {
			EditorUtility.DisplayDialog ("No selection", "Please select at least one object to generate an animator wrapper class for.", "OK");
			return false;
		}
		animator = Selection.activeGameObject.GetComponent<Animator> ();
		if (animator == null) {
			EditorUtility.DisplayDialog ("No Animator", "Please a game object that contains an animator compnent.", "OK");
			return false;
		}
		userSelection = Selection.activeGameObject.name;
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		int layerCount = ac.layerCount;
		for (int layer = 0; layer < layerCount; layer++) {
			string layerName = ac.GetLayer (layer).name;
			layerNames.Add (layerName);
			LayerStatesList current = new LayerStatesList (layer, layerName);
			UnityEditorInternal.StateMachine sm = ac.GetLayer (layer).stateMachine;
			for (int i = 0; i < sm.stateCount; i++) {
				UnityEditorInternal.State state = sm.GetState (i);
				string s = state.uniqueName;
				current.LayerStates.Add (s);
			}
			allLayerStates.Add (current);
		}
		targetCodeFile = userSelection + targetCodeFile;
		targetCodeFile = EditorUtility.SaveFilePanel ("Generate C# Code for Animator Wrapper Class", resourcesDir, targetCodeFile, "cs");
		if (targetCodeFile == null || targetCodeFile == "") {
			return false;
		}
		targetClassName = System.IO.Path.GetFileNameWithoutExtension (targetCodeFile);
		return true;
	}

	static void RegisterExistingNames () {
		Assembly assemblyCSharp = Assembly.Load ("Assembly-CSharp");
		try {
			Type t = assemblyCSharp.GetType (targetClassName);
			if (t == null) {
				return;
			}
			System.Object obj = assemblyCSharp.CreateInstance (targetClassName);
			const BindingFlags propBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
			PropertyInfo[] propertyInfos = obj.GetType().GetProperties(propBinding);
			foreach (PropertyInfo propertyInfo in propertyInfos) {
				previousMembers.Add (propertyInfo.Name);
			}
			const BindingFlags methodBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.InvokeMethod;
			MethodInfo[] methodInfos = obj.GetType ().GetMethods (methodBinding);
			foreach (MethodInfo methodInfo in methodInfos) {
				if (methodInfo.Name.StartsWith ("Is")) {
					previousMembers.Add (methodInfo.Name);
				}

			}
		} catch (System.Exception ex) {
			Debug.LogWarning (ex.Message + "\n" + ex.StackTrace);
		}
	}

	static string GenerateStateName (string prefix, string item, string layerPrefix) {
		string propName = item;
		if (!String.IsNullOrEmpty (layerPrefix) && !forceLayerPrefix) {
			int i = propName.IndexOf (".");
			if (i >= 0) {
				propName = propName.Substring (i + 1);
			}
		}
		return cg.GeneratePropertyName (prefix, propName);
	}
	
	static void WriteCodeToFile () {
		using (StreamWriter writer = new StreamWriter (targetCodeFile, false)) {
			try {
				writer.WriteLine ("{0}", code);
				Debug.Log ("Code written to file " + targetCodeFile);
				// both methods trigger occasionally an IOException: Sharing violation on path ...
//				EditorApplication.ExecuteMenuItem ("Assets/Sync MonoDevelop Project");
//				AssetDatabase.Refresh (ImportAssetOptions.Default);
				return;
			} catch (System.Exception ex) {
				string msg = " threw:\n" + ex.ToString ();
				Debug.LogError (msg);
				EditorUtility.DisplayDialog ("Error on export", msg, "OK");
			}
		}
	}

}

