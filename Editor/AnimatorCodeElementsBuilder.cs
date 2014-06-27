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
using AnimatorAccess;


namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// Analyses Animator and AnimatorController components for building a new ClassCodeElement. Note that calls to
	/// Unity internals which are encapsulated in class InternalAPIAccess are triggered from here.
	/// </summary>
	public class AnimatorCodeElementsBuilder : CodeElementsBuilder
	{
		const string StateInfoDict = "StateInfos";
		const string TransitionInfoDict = "TransitionInfos";

		Config config;
		string targetClassName;
		
		Animator animator;

		ClassCodeElement classCodeElement;
		/// <summary>
		/// The initialiser code i.e. Awake method. Future releases might contain an option to generate a plain class 
		/// version.
		/// </summary>
		protected ICodeBlock AwakeMethod;

		ICodeBlock EventManagerInitialiser;

		Dictionary<int, TransitionInfo> TransitionInfos = new Dictionary<int, TransitionInfo> ();

		public int allTransitionsHash = 0;

		public AnimatorCodeElementsBuilder (GameObject go, string className, Config c)
		{
			this.targetClassName = className;
			this.config = c;
			animator = go.GetComponent<Animator> ();
		}
		
		public ClassCodeElement Build () {
			classCodeElement = new ClassCodeElement (targetClassName);
			classCodeElement.Summary.Add ("Convenience class to access Animator states and parameters.");
			classCodeElement.Summary.Add ("Edits will be lost when this class is regenerated. ");
			classCodeElement.Summary.Add ("Hint: Editing might be useful after renaming animator items in complex projects:");
			classCodeElement.Summary.Add (" - Right click on an obsolete member and select Refactor/Rename. ");
			classCodeElement.Summary.Add (" - Change it to the new name. ");
			classCodeElement.Summary.Add (" - Delete this member to avoid comile error CS0102 ... already contains a definition ...''. ");
			string versionString = "" + DateTime.Now;
			classCodeElement.AddAttribute (new GeneratedClassAttributeCodeElement (versionString));
			if (!string.IsNullOrEmpty (config.DefaultNamespace)) {
				classCodeElement.NameSpace = new NameSpaceCodeElement (config.DefaultNamespace);
			}
			ProcessInternalFields ();
			if (config.GenerateMonoBehaviourComponent) {
				classCodeElement.SetBaseClass (config.MonoBehaviourComponentBaseClass);
				AwakeMethod = PrepareAwakeMethod ();
			} else {
				AwakeMethod = PrepareConstructors ();
			}
			EventManagerInitialiser = PrepareEventManagerInitialiserMethod ();
			ProcessAnimatorStates ();
			ProcessAnimatorParameters ();
			ProcessStateEventHandling ();
			return classCodeElement;
		}

		/// <summary>
		/// Depending on config setting a FixedUpdate or Update method will be generated. None will omit this step.
		/// </summary>
		void ProcessStateEventHandling () {
			StateEventHandlingMethod stateEventMethod = config.GenerateStateEventHandler;
//			StateEventHandlingMethod stateEventMethod = StateEventHandlingMethod.FixedUpdate;
			switch (stateEventMethod) {
			case StateEventHandlingMethod.FixedUpdate:
				GenericMethodCodeElement methodFixedUpdate = new GenericMethodCodeElement ("void", "FixedUpdate", AccessType.Private);
				methodFixedUpdate.Code.Add ("CheckForAnimatorStateChanges ();");
				classCodeElement.Methods.Add (methodFixedUpdate);
				break;
			case StateEventHandlingMethod.Update:
				GenericMethodCodeElement methodUpdate = new GenericMethodCodeElement ("void", "Update", AccessType.Private);
				methodUpdate.Code.Add ("CheckForAnimatorStateChanges (animator);");
				classCodeElement.Methods.Add (methodUpdate);
				break;
			default:
				return;
			}
			ProcessTransitions ();
		}

		void ProcessInternalFields () {
			// internal fields have moved to base class
		}

		/// <summary>
		/// Alpha code for feature to generate a plain class version that does initialisation in constructors.
		/// </summary>
		/// <returns>The constructors.</returns>
		ICodeBlock PrepareConstructors () {
			ConstructorCodeElement withAnimatorParam = new ConstructorCodeElement (targetClassName, "Animator animator");
			withAnimatorParam.Code.Add ("this.animator = animator;");
			classCodeElement.Constructors.Add (withAnimatorParam);
			ConstructorCodeElement defaultConstructor = new ConstructorCodeElement (targetClassName);
			defaultConstructor.Attributes.Add (new ObsoleteAttributeCodeElement ("Default constructor is provided only for internal use (reflection). Use " + targetClassName + " (Animator animator) instead", true));
			classCodeElement.Constructors.Add (defaultConstructor);
			return withAnimatorParam;
		}

		/// <summary>
		/// Prepares the awake method but does not add code to it. This will be done in the ProcessAnimatorXXX methods.
		/// </summary>
		/// <returns>The awake method.</returns>
		ICodeBlock PrepareAwakeMethod () {
			VoidMethodCodeElement method = new VoidMethodCodeElement ("Awake");
			method.Code.Add ("animator = GetComponent<Animator> ();");
			classCodeElement.Methods.Add (method);
			return method;
		}

		ICodeBlock PrepareEventManagerInitialiserMethod () {
			VoidMethodCodeElement method = new VoidMethodCodeElement ("InitialiseEventManager");
			method.overrideModifier = OverrideType.Override;
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

		/// <summary>
		/// Callback from InternalAPIAccess to process a single Animator state.
		/// </summary>
		/// <param name="layer">Layer index.</param>
		/// <param name="layerName">Layer name.</param>
		/// <param name="item">State name.</param>
		public void ProcessAnimatorState (StateInfo info) {
			string layerPrefix = (info.Layer > 0 || config.ForceLayerPrefix ? null : info.LayerName);
			string name = CodeGenerationUtils.GenerateStateName (config.AnimatorStatePrefix, info.Name, layerPrefix);
			name = name.FirstCharToUpper ();
			string fieldName = CodeGenerationUtils.GenerateStateName (config.AnimatorStateHashPrefix, info.Name, layerPrefix);
			fieldName = fieldName.FirstCharToLower ();
			// field declaration
			GenericFieldCodeElement field = new GenericFieldCodeElement (typeof(int), fieldName, "" + info.Id);
			field.ReadOnly = true;
			field.Summary.Add ("Hash of Animator state " + info.Name);
			classCodeElement.Fields.Add (field);
			// IsXXX method
			string methodName = "Is" + name;
			MethodCodeElement<bool> method = new MethodCodeElement<bool> (methodName);
			method.Origin = "state " + info.Name;
			method.AddParameter (typeof(int), "nameHash");
			method.Code.Add ("return nameHash == " + fieldName + ";");
			method.Summary.Add ("true if nameHash equals Animator.StringToHash (\"" + info.Name + "\").");
			classCodeElement.Methods.Add (method);
			// state dictionary is filled in overriden method InitialiseEventManager
			object [] parameters = new object[] {info.Id,
				info.Layer, 
				info.LayerName, 
				info.Name, 
				info.Tag, 
				info.Speed, 
				info.FootIK, 
				info.Mirror, 
				info.Motion.Name
			};
			string parameterList = CodeElementUtils.GetCallParameterString (parameters);
			EventManagerInitialiser.Code.Add (StateInfoDict + ".Add (" + info.Id + ", new StateInfo (" + parameterList + "));");
		}

		/// <summary>
		/// Generate code to fill transition info dictionary in InitialiseEventManager.
		/// </summary>
		void ProcessTransitions () {
			InternalAPIAccess.ProcessAllTransitions (animator, ProcessTransition);
			string allIds = "";
			foreach (int id in TransitionInfos.Keys) {
				allIds += ":" + id;
			}
			allTransitionsHash = allIds.GetHashCode ();
			PropertyCodeElement<int> p = new PropertyCodeElement<int> ("AllTransitionsHash");
			p.overrideModifier = OverrideType.Override;
			p.Getter.CodeLines.Add ("return " + allTransitionsHash + ";");
			classCodeElement.Properties.Add (p);
		}

		void ProcessTransition (TransitionInfo info) {
			TransitionInfos.Add (info.Id, info);
			object [] parameters = new object[] {info.Id,
				info.Name, 
				info.Layer, 
				info.LayerName, 
				info.SourceId, 
				info.DestId, 
				info.Atomic, 
				info.Duration, 
				info.Mute,
				info.Offset,
				info.Solo,
			};
			string parameterList = CodeElementUtils.GetCallParameterString (parameters);
			EventManagerInitialiser.Code.Add (TransitionInfoDict + ".Add (" + info.Id + ", new TransitionInfo (" + parameterList + "));");
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
			GenericFieldCodeElement field = new FieldCodeElement<int> (fieldName, "" + Animator.StringToHash (item));
			field.ReadOnly = true;
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
			classCodeElement.Fields.Add (field);
		}

	}
}

