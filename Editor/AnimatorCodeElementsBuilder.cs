// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Reflection;
using Scio.CodeGeneration;


namespace Scio.AnimatorWrapper
{
	public class AnimatorCodeElementsBuilder : CodeElementsBuilder
	{
		Config config;
		string targetClassName;
		
		Animator animator;
		
		ClassCodeElement classCodeElement;
		
		public AnimatorCodeElementsBuilder (GameObject go, string className, Config c)
		{
			this.targetClassName = className;
			this.config = c;
			animator = go.GetComponent<Animator> ();
		}
		
		public ClassCodeElement Build () {
			classCodeElement = new ClassCodeElement (targetClassName);
			// TODO_kay: fix test code
//			classCodeElement.SetBaseClass ("MonoBehaviour");
//			classCodeElement.AddInterface ("DummyCodeGenerator");
//			classCodeElement.AddInterface ("DummyTest");
//			classCodeElement.AddInterface ("DummyThird");
			classCodeElement.Summary.Add ("Convenience class to access Animator states and parameters.");
			classCodeElement.Summary.Add ("DON'T EDIT! Your changes will be lost when this class is regenerated.");
			string versionString = "" + DateTime.Now;
			classCodeElement.AddAttribute (new GeneratedClassAttributeCodeElement (versionString));
			if (!string.IsNullOrEmpty (config.DefaultNamespace)) {
				classCodeElement.NameSpace = new NameSpaceCodeElement (config.DefaultNamespace);
			}
			PrepareVariables ();
			if (config.GenerateMonoBehaviourComponent) {
				classCodeElement.SetBaseClass (config.MonoBehaviourComponentBaseClass);
				PrepareAwakeMethod ();
			} else {
				PrepareConstructors ();
			}
			PrepareMethods ();
			PrepareProperties ();
			return classCodeElement;
		}
		
		void PrepareVariables () {
			GenericVariableCodeElement animatorVar = new GenericVariableCodeElement ("Animator", "animator", "", AccessType.Private);
			classCodeElement.Variables.Add (animatorVar);
		}
		
		void PrepareConstructors () {
			ConstructorCodeElement withAnimatorParam = new ConstructorCodeElement (targetClassName, "Animator animator");
			withAnimatorParam.Code.Add ("this.animator = animator;");
			classCodeElement.Constructors.Add (withAnimatorParam);
			ConstructorCodeElement defaultConstructor = new ConstructorCodeElement (targetClassName);
			defaultConstructor.Attributes.Add (new ObsoleteAttributeCodeElement ("Default constructor is provided only for internal use (reflection). Use " + targetClassName + " (Animator animator) instead", true));
			classCodeElement.Constructors.Add (defaultConstructor);
		}

		void PrepareAwakeMethod () {
			GenericMethodCodeElement method = new GenericMethodCodeElement ("void", "Awake");
			method.Code.Add ("animator = GetComponent<Animator> ();");
			classCodeElement.Methods.Add (method);
		}
		
		void PrepareMethods () {
			UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
			int layerCount = ac.layerCount;
			for (int layer = 0; layer < layerCount; layer++) {
				string layerName = ac.GetLayer (layer).name;
				UnityEditorInternal.StateMachine sm = ac.GetLayer (layer).stateMachine;
				for (int i = 0; i < sm.stateCount; i++) {
					UnityEditorInternal.State state = sm.GetState (i);
					string item = state.uniqueName;
					int nameHash = Animator.StringToHash (item);
					string propName = GenerateStateName (config.AnimatorStatePrefix, item, (layer > 0 ? null : layerName));
					string methodName = "Is" + propName;
					MethodCodeElement<bool> method = new MethodCodeElement<bool> (methodName);
					method.Origin = "state " + item;
					method.AddParameter (typeof(int), "nameHash");
					method.Code.Add (" return nameHash == " + nameHash + ";");
					method.Summary.Add ("true if nameHash equals Animator.StringToHash (" + item + ").");
					classCodeElement.Methods.Add (method);
				}
			}
		}
		
		void PrepareProperties () {
			AnimatorController animatorController = AnimatorController.GetEffectiveAnimatorController (animator);
			int countParameters = animatorController.parameterCount;
			if (countParameters > 0) {
				for (int i = 0; i < countParameters; i++) {
					AnimatorControllerParameter parameter = animatorController.GetParameter (i);
					int paramHash = Animator.StringToHash (parameter.name);
					string propName = CodeGenerationUtils.GeneratePropertyName (config.ParameterPrefix, parameter.name);
					GenericPropertyCodeElement type = null;
					if (parameter.type == AnimatorControllerParameterType.Bool) {
						type = new PropertyCodeElement<bool> (propName);
						type.Summary.Add ("Access to parameter " + parameter.name + ", default: " + parameter.defaultBool);
						type.Getter.CodeLines.Add ("return animator.GetBool (" + paramHash + ");");
						type.Setter.CodeLines.Add ("animator.SetBool (" + paramHash + ", value);");
						classCodeElement.Properties.Add (type);
					} else if (parameter.type == AnimatorControllerParameterType.Float) {
						type = new PropertyCodeElement<float> (propName);
						type.Summary.Add ("Access to parameter " + parameter.name + ", default: " + parameter.defaultFloat);
						type.Getter.CodeLines.Add ("return animator.GetFloat (" + paramHash + ");");
						type.Setter.CodeLines.Add ("animator.SetFloat (" + paramHash + ", value);");
						classCodeElement.Properties.Add (type);
					} else if (parameter.type == AnimatorControllerParameterType.Int) {
						type = new PropertyCodeElement<int> (propName);
						type.Summary.Add ("Access to parameter " + parameter.name + ", default: " + parameter.defaultInt);
						type.Getter.CodeLines.Add ("return animator.GetInteger (" + paramHash + ");");
						type.Setter.CodeLines.Add ("animator.SetInteger (" + paramHash + ", value);");
						classCodeElement.Properties.Add (type);
					} else if (parameter.type == AnimatorControllerParameterType.Trigger) {
						type = new PropertyCodeElement<bool> (propName);
						type.Summary.Add ("Access to parameter " + parameter.name);
						type.Setter.CodeLines.Add ("animator.SetTrigger (" + paramHash + ");");
						classCodeElement.Properties.Add (type);
					} else {
						Debug.LogWarning ("Could not find type for param " + parameter + " as it seems to be no base type.");
					}
					if (type != null) {
						type.Origin = "parameter " + parameter;
					}
				}
			}
		}
		
		string GenerateStateName (string prefix, string item, string layerPrefix)
		{
			string propName = item;
			if (!String.IsNullOrEmpty (layerPrefix) && !config.ForceLayerPrefix) {
				int i = propName.IndexOf (layerPrefix + ".");
				if (i >= 0) {
					propName = propName.Substring (layerPrefix.Length + 1);
				} else {
					Debug.LogWarning ("Item [" + item + "] does not contain [" + layerPrefix + "] as prefix");
				}
			}
			return CodeGenerationUtils.GeneratePropertyName (prefix, propName);
		}
		
	}
}

