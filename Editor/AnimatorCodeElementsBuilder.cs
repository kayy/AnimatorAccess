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
			GenericMethodCodeElement method = new GenericMethodCodeElement ("void", "Awake");
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
			InternalAPIAccess.ProcessAnimatorParameters (animator, classCodeElement, config.ParameterPrefix);
		}
		

	}
}

