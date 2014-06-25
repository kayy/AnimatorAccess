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
using AnimatorAccess;

namespace Scio.AnimatorAccessGenerator
{
	public enum AnimatorParameterType
	{
		Unknown = -1,
		Int = 0,
		Float = 1,
		Bool = 2,
		Trigger = 3,
	}
	
	/// <summary>
	/// Encapsulates all critical access to UnityEditorInternal stuff. Methods within this class might be affected
	/// by future changes of the Unity API. Thus preprocessor #if statements are expected to grow.
	/// </summary>
	public static class InternalAPIAccess
	{
		public delegate void ProcessAnimatorState (StateInfo info);

		public delegate void ProcessAnimatorTransition (TransitionInfo info);

		public delegate void ProcessAnimatorParameter (AnimatorParameterType t, string item, string defaultValue);

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
					StateInfo info = new StateInfo (state.uniqueNameHash, layer, layerName, state.uniqueName, state.tag,
						state.speed, state.iKOnFeet, state.mirror, state.GetMotion ().name);
					callback (info);
				}
			}
		}

		public static void ProcessAllTransitions (Animator animator, ProcessAnimatorTransition callback) {
			AnimatorController controller = GetInternalAnimatorController (animator);
			int layerCount = controller.layerCount;
			for (int layer = 0; layer < layerCount; layer++) {
				string layerName = controller.GetLayer (layer).name;
				UnityEditorInternal.StateMachine sm = controller.GetLayer (layer).stateMachine;
				for (int i = 0; i < sm.stateCount; i++) {
					UnityEditorInternal.State state = sm.GetState (i);
					Transition[] transitions = sm.GetTransitionsFromState(state);
					foreach (var t in transitions) {
//						Debug.Log (state.uniqueName +  ", transition: " + t.uniqueName + " ---" + " dest = " + t.dstState + " (" + (Animator.StringToHash (state.uniqueName) == Animator.StringToHash (layerName + "." + t.dstState)) + ") " + " src = " + t.srcState);
						TransitionInfo info = new TransitionInfo (t.uniqueNameHash, t.uniqueName, layer, layerName, 
	                        t.srcState.uniqueNameHash, t.dstState.uniqueNameHash, t.atomic, t.duration, t.mute, t.offset, t.solo);
						callback (info);
					}
				}
			}
		}

		/// <summary>
		/// Adds all Animator parameters as properties to the class code element. NOTE that this method relies on 
		/// classes from namespace UnityEditorInternal which can be subject to changes in future releases.
		/// </summary>
		/// <param name="animator">Animator instance to inspect.</param>
		/// <param name="callback">Callback delegate for processing each of the single paramters.</param>
		public static void ProcessAnimatorParameters (Animator animator, ProcessAnimatorParameter callback)
		{
			AnimatorController controller = GetInternalAnimatorController (animator);
			int countParameters = controller.parameterCount;
			if (countParameters > 0) {
				for (int i = 0; i < countParameters; i++) {
					AnimatorControllerParameter parameter = controller.GetParameter (i);
					string item = parameter.name;
					if (parameter.type == AnimatorControllerParameterType.Bool) {
						callback (AnimatorParameterType.Bool, item, "" + parameter.defaultBool);
					} else if (parameter.type == AnimatorControllerParameterType.Float) {
						callback (AnimatorParameterType.Float, item, "" + parameter.defaultFloat);
					} else if (parameter.type == AnimatorControllerParameterType.Int) {
						callback (AnimatorParameterType.Int, item, "" + parameter.defaultInt);
					} else if (parameter.type == AnimatorControllerParameterType.Trigger) {
						callback (AnimatorParameterType.Trigger, item, "(not available)");
					} else {
						callback (AnimatorParameterType.Unknown, item, null);
					}
				}
			}
		}

	}
}

