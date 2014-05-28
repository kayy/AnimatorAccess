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
using Scio.CodeGeneration;

namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// Encapsulates all critical access to UnityEditorInternal stuff. Methods within this class might be affected
	/// by future changes of the Unity API. Thus preprocessor #if statements are expected to grow.
	/// </summary>
	public static class InternalAPIAccess
	{
		public delegate void ProcessAnimatorState (int layer, string layerName, string item);
		static AnimatorController GetInternalAnimatorController (Animator animator) {
			return animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		}

		/// <summary>
		/// Adds convenience methods to determine the current Animator state as boolean methods to the class code 
		/// element (e.g. IsIdle ()). NOTE that this method relies on classes from namespace UnityEditorInternal 
		/// which can be subject to changes in future releases.
		/// </summary>
		public static void ProcessAllAnimatorStates (Animator animator, ProcessAnimatorState callback) {
			AnimatorController controller = GetInternalAnimatorController (animator);
			int layerCount = controller.layerCount;
			for (int layer = 0; layer < layerCount; layer++) {
				string layerName = controller.GetLayer (layer).name;
				UnityEditorInternal.StateMachine sm = controller.GetLayer (layer).stateMachine;
				for (int i = 0; i < sm.stateCount; i++) {
					UnityEditorInternal.State state = sm.GetState (i);
					string item = state.uniqueName;
					callback (layer, layerName, item);
				}
			}
		}
		
		/// <summary>
		/// Adds all Animator parameters as properties to the class code element. NOTE that this method relies on 
		/// classes from namespace UnityEditorInternal which can be subject to changes in future releases.
		/// </summary>
		public static void ProcessAnimatorParameters (Animator animator, ClassCodeElement classCodeElement, string parameterPrefix)
		{
			AnimatorController controller = GetInternalAnimatorController (animator);
			int countParameters = controller.parameterCount;
			if (countParameters > 0) {
				for (int i = 0; i < countParameters; i++) {
					AnimatorControllerParameter parameter = controller.GetParameter (i);
					int paramHash = Animator.StringToHash (parameter.name);
					string propName = CodeGenerationUtils.GeneratePropertyName (parameterPrefix, parameter.name);
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
						Logger.Warning ("Could not find type for param " + parameter + " as it seems to be no base type.");
					}
					if (type != null) {
						type.Origin = "parameter " + parameter;
					}
				}
			}
		}

	}
}

