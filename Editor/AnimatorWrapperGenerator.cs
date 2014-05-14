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
	AnimatorWrapperGeneratorConfig config;
	string targetClassName;
	
	GameObject selectedGameObject;
	Animator animator;
	
	List<LayerStatesList> allLayerStates = new List<LayerStatesList> ();
	List<string> previousMembers = new List<string> ();

	ClassCodeElement classCodeElement;

	string code = "";
	public string Code {
		get { return code; }
	}

	CodeGenerationUtils cg = new CodeGenerationUtils ();

	public AnimatorWrapperGenerator (GameObject go, string fileName)
	{
		selectedGameObject = go;
		animator = selectedGameObject.GetComponent<Animator> ();
		targetClassName = System.IO.Path.GetFileNameWithoutExtension (fileName);
		config = AnimatorWrapperGeneratorConfigFactory.Get (targetClassName);
		Prepare ();
		Debug.Log (classCodeElement);
		RegisterExistingNames ();
	}
	
	public bool Generate () {
		GenerateClass ();
		if ((previousMembers.Count > 0)) {
			string removedMembers = "";
			string consoleMessage = "";
			int count = previousMembers.Count;
			for (int i = 0; i < count; i++) {
				consoleMessage += previousMembers [i] + "\n";
				if (i < 3) {
					removedMembers += previousMembers [i] + "\n";
				}
				else
					if (i >= count - 1) {
						removedMembers += "... (" + (count - 3) + " more)\n";
					}
			}
			Debug.Log ("Members found in previous version that disappeared now: " + consoleMessage);
			string s = string.Format ("The following members are found in the previous version of {0} but will not be " + "created again:\n{1}\n(See console for details)\nClick 'OK' to generate new version. Click 'Cancel' if you want" + " to refactor your code first if other classes refer to these members.", targetClassName, removedMembers);
			if (!EditorUtility.DisplayDialog (previousMembers.Count + " Removed Members", s, "OK", "Cancel")) {
				Debug.Log ("Code generation cancelled for class " + targetClassName + ". The generated code would have been:\n" + code);
				return false;
			}
		}
		return true;
	}
	
	void GenerateClass () {
		string versionInfo = System.DateTime.Now  + ", selected game object was " + selectedGameObject.name;
		code = cg.GenerateMITHeader (versionInfo);
		code += cg.Code (0, "using UnityEngine");
		code += cg.Code (0, "using System.Collections.Generic", 2);
		code += cg.MakeComment (0, "Provides convenient access to parameters and states of the Animator class." +
		                                ""
		                                );
		code += cg.Line (0, "public partial class " + targetClassName);
		code += cg.Line (0, "{");
		code += cg.Code (1, "const string VersionInfo = \"" + versionInfo + "\"", 2);
		AddClassCode ();
		code += cg.Line (0, "}");
	}

	void AddClassCode () {
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
				string propName = GenerateStateName (config.AnimationStatePrefix, item, (layerStates.LayerIndex > 0 ? null : layerStates.LayerName));
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
				string propName = cg.GeneratePropertyName (config.ParameterPrefix, parameter.name);
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

	void RegisterMember (string member) {
		if (previousMembers.Contains (member)) {
			previousMembers.Remove (member);
		}
	}

	void Prepare () {
		classCodeElement = new ClassCodeElement (targetClassName);
		PrepareVariables ("DUMMY VERSION");
		PrepareConstructors ();
		PrepareMethods ();
		PrepareProperties ();
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		int layerCount = ac.layerCount;
		for (int layer = 0; layer < layerCount; layer++) {
			string layerName = ac.GetLayer (layer).name;
			LayerStatesList current = new LayerStatesList (layer, layerName);
			UnityEditorInternal.StateMachine sm = ac.GetLayer (layer).stateMachine;
			for (int i = 0; i < sm.stateCount; i++) {
				UnityEditorInternal.State state = sm.GetState (i);
				string item = state.uniqueName;
				current.LayerStates.Add (item);
			}
			allLayerStates.Add (current);
		}
	}

	public void PrepareVariables (string versionInfo) {
		VariableCodeElement<string> version = new VariableCodeElement<string> ("VersionInfo", "\"" + versionInfo + "\"");
		version.Const = true;
		classCodeElement.Variables.Add (version);
		GenericVariableCodeElement animator = new GenericVariableCodeElement ("Animator", "animator", "", AbstractCodeElement.AccessType.Private);
		classCodeElement.Variables.Add (animator);
	}
	
	public void PrepareConstructors () {
		ConstructorCodeElement withAnimatorParam = new ConstructorCodeElement (targetClassName, "Animator animator");
		withAnimatorParam.Code.Add ("this.animator = animator;");
		classCodeElement.Constructors.Add (withAnimatorParam);
		ConstructorCodeElement defaultConstructor = new ConstructorCodeElement (targetClassName);
		defaultConstructor.Obsolete = true;
		defaultConstructor.ErrorOnObsolete = true;
		defaultConstructor.ObsoleteMessage = "Default constructor is provided only for internal use (reflection). Use " + targetClassName + " (Animator animator) instead";
		classCodeElement.Constructors.Add (defaultConstructor);
	}
	
	public void PrepareMethods () {
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		int layerCount = ac.layerCount;
		for (int layer = 0; layer < layerCount; layer++) {
			string layerName = ac.GetLayer (layer).name;
			UnityEditorInternal.StateMachine sm = ac.GetLayer (layer).stateMachine;
			for (int i = 0; i < sm.stateCount; i++) {
				UnityEditorInternal.State state = sm.GetState (i);
				string item = state.uniqueName;
				int nameHash = Animator.StringToHash (item);
				string propName = GenerateStateName (config.AnimationStatePrefix, item, (layer > 0 ? null : layerName));
				string methodName = "Is" + propName;
				MethodCodeElement<bool> method = new MethodCodeElement<bool> (methodName);
				method.Parameters = "int nameHash";
				method.Code.Add (" return nameHash == " + nameHash + ";");
				method.Summary.Add ("true if nameHash equals Animator.StringToHash (" + item + ").");
				classCodeElement.Methods.Add (method);
			}
		}
	}

	public void PrepareProperties () {
		AnimatorController animatorController = AnimatorController.GetEffectiveAnimatorController(animator);
		int countParameters = animatorController.parameterCount;
		if (countParameters > 0) {
			for (int i = 0; i < countParameters; i++) {
				AnimatorControllerParameter parameter = animatorController.GetParameter (i);
				int paramHash = Animator.StringToHash (parameter.name);
				string propName = cg.GeneratePropertyName (config.ParameterPrefix, parameter.name);
				if (parameter.type == AnimatorControllerParameterType.Bool) {
					GenericPropertyCodeElement type = new PropertyCodeElement<bool> (propName);
					type.Summary.Add ("Access to parameter " + parameter.name + ", default: " + parameter.defaultBool);
					type.GetterCode.Add ("return animator.GetBool (" + paramHash + ");");
					type.SetterCode.Add ("animator.SetBool (" + paramHash + ", value);");
					classCodeElement.Properties.Add (type);
				} else if (parameter.type == AnimatorControllerParameterType.Float) {
					GenericPropertyCodeElement type = new PropertyCodeElement<float> (propName);
					type.Summary.Add ("Access to parameter " + parameter.name + ", default: " + parameter.defaultFloat);
					type.GetterCode.Add ("return animator.GetFloat (" + paramHash + ");");
					type.SetterCode.Add ("animator.SetFloat (" + paramHash + ", value);");
					classCodeElement.Properties.Add (type);
				} else if (parameter.type == AnimatorControllerParameterType.Int) {
					GenericPropertyCodeElement type = new PropertyCodeElement<int> (propName);
					type.Summary.Add ("Access to parameter " + parameter.name + ", default: " + parameter.defaultInt);
					type.GetterCode.Add ("return animator.GetInteger (" + paramHash + ");");
					type.SetterCode.Add ("animator.SetInteger (" + paramHash + ", value);");
					classCodeElement.Properties.Add (type);
				} else if (parameter.type == AnimatorControllerParameterType.Trigger) {
					GenericPropertyCodeElement type = new PropertyCodeElement<bool> (propName);
					type.Summary.Add ("Access to parameter " + parameter.name);
					type.GetterCode.Add ("return animator.SetTrigger (" + paramHash + ");");
					type.SetterCode.Add ("animator.SetBool (" + paramHash + ", value);");
					classCodeElement.Properties.Add (type);
				}
			}
		}
	}
	void RegisterExistingNames () {
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

	string GenerateStateName (string prefix, string item, string layerPrefix) {
		string propName = item;
		if (!String.IsNullOrEmpty (layerPrefix) && !config.ForceLayerPrefix) {
			int i = propName.IndexOf (".");
			if (i >= 0) {
				propName = propName.Substring (i + 1);
			}
		}
		return cg.GeneratePropertyName (prefix, propName);
	}
	
}

