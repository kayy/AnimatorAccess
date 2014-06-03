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
using System.Collections.Generic;
using System.Reflection;
using Scio.CodeGeneration;


namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// Analyses Animator and AnimatorController components for building a new ClassCodeElement. Note that calls to
	/// Unity internals which are encapsulated in class InternalAPIAccess are triggered from here.
	/// </summary>
	public class AnimatorCodeElementsBuilder : CodeElementsBuilder
	{
		Config config;
		string targetClassName;
		
		Animator animator;
		
		ClassCodeElement classCodeElement;

		ICodeBlock initialiserCode;
		
		public AnimatorCodeElementsBuilder (GameObject go, string className, Config c)
		{
			this.targetClassName = className;
			this.config = c;
			animator = go.GetComponent<Animator> ();
		}
		
		public ClassCodeElement Build () {
			classCodeElement = new ClassCodeElement (targetClassName);
			classCodeElement.Summary.Add ("Convenience class to access Animator states and parameters.");
			classCodeElement.Summary.Add ("DON'T EDIT! Your changes will be lost when this class is regenerated.");
			string versionString = "" + DateTime.Now;
			classCodeElement.AddAttribute (new GeneratedClassAttributeCodeElement (versionString));
			if (!string.IsNullOrEmpty (config.DefaultNamespace)) {
				classCodeElement.NameSpace = new NameSpaceCodeElement (config.DefaultNamespace);
			}
			PrepareFields ();
			if (config.GenerateMonoBehaviourComponent) {
				classCodeElement.SetBaseClass (config.MonoBehaviourComponentBaseClass);
				initialiserCode = PrepareAwakeMethod ();
			} else {
				initialiserCode = PrepareConstructors ();
			}
			ProcessAnimatorStates ();
			ProcessAnimatorParameters ();
			return classCodeElement;
		}

		void PrepareFields () {
			GenericFieldCodeElement animatorVar = new GenericFieldCodeElement ("Animator", "animator", "", AccessType.Private);
			classCodeElement.Fields.Add (animatorVar);
		}
		
		ICodeBlock PrepareConstructors () {
			ConstructorCodeElement withAnimatorParam = new ConstructorCodeElement (targetClassName, "Animator animator");
			withAnimatorParam.Code.Add ("this.animator = animator;");
			classCodeElement.Constructors.Add (withAnimatorParam);
			ConstructorCodeElement defaultConstructor = new ConstructorCodeElement (targetClassName);
			defaultConstructor.Attributes.Add (new ObsoleteAttributeCodeElement ("Default constructor is provided only for internal use (reflection). Use " + targetClassName + " (Animator animator) instead", true));
			classCodeElement.Constructors.Add (defaultConstructor);
			return withAnimatorParam;
		}

		ICodeBlock PrepareAwakeMethod () {
			VoidMethodCodeElement method = new VoidMethodCodeElement ("Awake");
			method.Code.Add ("animator = GetComponent<Animator> ();");
			classCodeElement.Methods.Add (method);
			return method;
		}

		/// <summary>
		/// Adds convenience methods to determine the current Animator state as boolean methods to the class code 
		/// element (e.g. IsIdle ()). NOTE that this method relies on classes from namespace UnityEditorInternal 
		/// which can be subject to changes in future releases.
		/// </summary>
		void ProcessAnimatorStates () {
			InternalAPIAccess.ProcessAllAnimatorStates (animator, ProcessAnimatorState);
		}

		public void ProcessAnimatorState (int layer, string layerName, string item) {
			string layerPrefix = (layer > 0 || config.ForceLayerPrefix ? null : layerName);
			string name = CodeGenerationUtils.GenerateStateName (config.AnimatorStatePrefix, item, layerPrefix);
			name = name.FirstCharToUpper ();
			string fieldName = CodeGenerationUtils.GenerateStateName (config.AnimatorStateHashPrefix, item, layerPrefix);
			fieldName = fieldName.FirstCharToLower ();
			GenericFieldCodeElement field = new GenericFieldCodeElement (typeof(int), fieldName);
			field.Summary.Add ("Hash of Animator state " + item);
			classCodeElement.Fields.Add (field);
			initialiserCode.Code.Add (fieldName + " = Animator.StringToHash (\"" + item + "\");");
			string methodName = "Is" + name;
			MethodCodeElement<bool> method = new MethodCodeElement<bool> (methodName);
			method.Origin = "state " + item;
			method.AddParameter (typeof(int), "nameHash");
			method.Code.Add (" return nameHash == " + fieldName + ";");
			method.Summary.Add ("true if nameHash equals Animator.StringToHash (\"" + item + "\").");
			classCodeElement.Methods.Add (method);
		}

		/// <summary>
		/// Adds all Animator parameters as properties to the class code element. NOTE that this method relies on 
		/// classes from namespace UnityEditorInternal which can be subject to changes in future releases.
		/// </summary>
		void ProcessAnimatorParameters () {
			InternalAPIAccess.ProcessAnimatorParameters (animator, ProcessAnimatorParameter);
		}

		/// <summary>
		/// Callback from InternalAPIAccess to process a single parameter. Hopefully this remains stable on API changes.
		/// </summary>
		/// <param name="t">Type of parameter in our representation.</param>
		/// <param name="item">Raw parameter name.</param>
		/// <param name="defaultValue">Default value.</param>
		public void ProcessAnimatorParameter (AnimatorParameterType t, string item, string defaultValue) {
			string propName = CodeGenerationUtils.GeneratePropertyName (config.ParameterPrefix, item);
			string getterName = "Get" + propName;
			string setterName = "Set" + propName;
			string fieldName = CodeGenerationUtils.GeneratePropertyName (config.ParameterHashPrefix, item);
			fieldName = fieldName.FirstCharToLower ();
			VoidMethodCodeElement setterMethod = new VoidMethodCodeElement (setterName);
			GenericMethodCodeElement getterMethod = null;
			GenericFieldCodeElement field = new FieldCodeElement<int> (fieldName, "");
			if (t == AnimatorParameterType.Bool) {
				getterMethod = new MethodCodeElement<bool> (getterName);
				getterMethod.Summary.Add ("Access to boolean parameter " + item + ", default is: " + defaultValue + ".");
				getterMethod.Code.Add ("return animator.GetBool (" + fieldName + ");");
				setterMethod.Summary.Add ("Set boolean value of parameter " + item + ".");
				setterMethod.Summary.Add ("<param name=\"newValue\">New value for boolean parameter " + item + ".</param>");
				setterMethod.AddParameter (typeof (bool), "newValue");
				setterMethod.Code.Add ("animator.SetBool (" + fieldName + ", newValue);");
			} else if (t == AnimatorParameterType.Float) {
				getterMethod = new MethodCodeElement<float> (getterName);
				getterMethod.Summary.Add ("Access to float parameter " + item + ", default is: " + defaultValue + ".");
				getterMethod.Code.Add ("return animator.GetFloat (" + fieldName + ");");
				setterMethod.Summary.Add ("Set float value of parameter " + item + ".");
				setterMethod.Summary.Add ("<param name=\"newValue\">New value for float parameter " + item + ".</param>");
				setterMethod.AddParameter (typeof (float), "newValue");
				setterMethod.Code.Add ("animator.SetFloat (" + fieldName + ", newValue);");
				// special case for float: provide setter with dampTime and deltaTime
				VoidMethodCodeElement methodExtended = new VoidMethodCodeElement (setterName);
				methodExtended.AddParameter (typeof (float), "newValue");
				methodExtended.AddParameter (typeof (float), "dampTime");
				methodExtended.AddParameter (typeof (float), "deltaTime");
				methodExtended.Summary.Add ("Set float parameter of " + item + " using damp and delta time .");
				methodExtended.Summary.Add ("<param name=\"newValue\">New value for float parameter " + item + ".</param>");
				methodExtended.Summary.Add ("<param name=\"dampTime\">The time allowed to parameter " + item + " to reach the value.</param>");
				methodExtended.Summary.Add ("<param name=\"deltaTime\">The current frame deltaTime.</param>");
				methodExtended.Code.Add ("animator.SetFloat (" + fieldName + ", newValue, dampTime, deltaTime);");
				classCodeElement.Methods.Add (methodExtended);
			} else if (t == AnimatorParameterType.Int) {
				getterMethod = new MethodCodeElement<int> (getterName);
				getterMethod.Summary.Add ("Access to integer parameter " + item + ", default is: " + defaultValue + ".");
				getterMethod.Code.Add ("return animator.GetInteger (" + fieldName + ");");
				setterMethod.Summary.Add ("Set integer value of parameter " + item + ".");
				setterMethod.Summary.Add ("<param name=\"newValue\">New value for integer parameter " + item + ".</param>");
				setterMethod.AddParameter (typeof (int), "newValue");
				setterMethod.Code.Add ("animator.SetInteger (" + fieldName + ", newValue);");
			} else if (t == AnimatorParameterType.Trigger) {
				setterMethod.Summary.Add ("Activate trigger of parameter " + item + ".");
				setterMethod.Code.Add ("animator.SetTrigger (" + fieldName + ");");
			} else {
				Logger.Warning ("Could not find type for param " + item + " as it seems to be no base type.");
				return;
			}
			classCodeElement.Methods.Add (setterMethod);
			if (getterMethod != null) {
				classCodeElement.Methods.Add (getterMethod);
			}
			field.Summary.Add ("Hash of parameter " + item);
			initialiserCode.Code.Add (fieldName + " = Animator.StringToHash (\"" + item + "\");");
			classCodeElement.Fields.Add (field);
		}

	}
}

