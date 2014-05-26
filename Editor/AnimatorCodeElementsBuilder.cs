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
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Reflection;
using Scio.CodeGeneration;


namespace Scio.AnimatorWrapper
{
	/// <summary>
	/// Analyses Animator and AnimatorController components for building a new ClassCodeElement.
	/// </summary>
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
				PrepareAwakeMethod ();
			} else {
				PrepareConstructors ();
			}
			PrepareMethods ();
			PrepareProperties ();
			return classCodeElement;
		}

		AnimatorController GetInternalAnimatorController () {
			return animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		}
		
		void PrepareFields () {
			GenericFieldCodeElement animatorVar = new GenericFieldCodeElement ("Animator", "animator", "", AccessType.Private);
			classCodeElement.Fields.Add (animatorVar);
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

		/// <summary>
		/// Adds convenience methods to determine the current Animator state as boolean methods to the class code 
		/// element (e.g. IsIdle ()). NOTE that this method relies on classes from namespace UnityEditorInternal 
		/// which can be subject to changes in future releases.
		/// </summary>
		void PrepareMethods () {
			InternalAPIAccess.PrepareMethods (animator, classCodeElement, config.AnimatorStatePrefix, config.ForceLayerPrefix);
		}

		/// <summary>
		/// Adds all Animator parameters as properties to the class code element. NOTE that this method relies on 
		/// classes from namespace UnityEditorInternal which can be subject to changes in future releases.
		/// </summary>
		void PrepareProperties () {
			InternalAPIAccess.PrepareProperties (animator, classCodeElement, config.ParameterPrefix);
		}
		

	}
}

