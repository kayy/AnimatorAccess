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
using System.Collections.Generic;


namespace AnimatorAccess {
	/// <summary>
	/// Base class for all generated AnimatorAccess classes.
	/// </summary>
	public class BaseAnimatorAccess : MonoBehaviour 
	{
		/// <summary>
		/// Callback method for animator state changes.
		/// <param name="layer">Layer in which the state has changed.</param>
		/// <param name="newState">New state just entered.</param>
		/// <param name="previousState">Previous state.</param>
		/// </summary>
		public delegate void OnStateChangeHandler (int layer, int newState, int previousState);

		public delegate void OnTransitionStartedHandler (TransitionInfo info);

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		public event OnStateChangeHandler OnStateChange;

		public event OnTransitionStartedHandler OnTransitionStarted;

		// TODO_kay: array? List<Handlers>
		Dictionary<int, TransitionEventHandler> transitionEventHandlers = new Dictionary<int, TransitionEventHandler> ();

		int [] _internalPreviousLayerStates;
		int [] _internalTransitions;

		int _internalLayerCount = -1;

		void Initialise (Animator animator) {
			_internalLayerCount = animator.layerCount;
			_internalPreviousLayerStates = new int[animator.layerCount];
			_internalTransitions = new int[animator.layerCount];
		}

		public TransitionEventHandler Transition (int src, int dest) {
			if (!transitionEventHandlers.ContainsKey (src)) {
				transitionEventHandlers [src] = new TransitionEventHandler ();
			}
			return transitionEventHandlers [src];
		}

		public TransitionEventHandler TransitionFrom (int src) {
			if (!transitionEventHandlers.ContainsKey (src)) {
				transitionEventHandlers [src] = new TransitionEventHandler ();
			}
			return transitionEventHandlers [src];
		}

		public TransitionEventHandler AnyTransition () {
			if (!transitionEventHandlers.ContainsKey (0)) {
				transitionEventHandlers [0] = new TransitionEventHandler ();
			}
			return transitionEventHandlers [0];
		}

		/// <summary>
		/// Checks for animator state changes if there are listeners registered in OnStateChange.
		/// </summary>
		/// <param name="animator">Animator instance for reading states of all layers.</param>
		public void CheckForAnimatorStateChanges (Animator animator) {
			if (OnStateChange != null) {
				if (_internalLayerCount < 0) {
					Initialise (animator);
				}
				for (int layer = 0; layer < _internalLayerCount; layer++) {
					int current = animator.GetCurrentAnimatorStateInfo (layer).nameHash;
					if (current != _internalPreviousLayerStates [layer]) {
						OnStateChange (layer, current, _internalPreviousLayerStates [layer]);
						_internalPreviousLayerStates [layer] = current;
					}
					
				}
			}
			if (OnTransitionStarted != null) {
				if (_internalLayerCount < 0) {
					Initialise (animator);
				}
				for (int layer = 0; layer < _internalLayerCount; layer++) {
					if (animator.IsInTransition (layer)) {
						AnimatorTransitionInfo animatorTransitionInfo = animator.GetAnimatorTransitionInfo (layer);
						int transitionNameHash = animatorTransitionInfo.nameHash;
						if (_internalTransitions [layer] != transitionNameHash) {
							TransitionInfo info = new TransitionInfo ();
							OnTransitionStarted (info);
							if (transitionEventHandlers.ContainsKey (transitionNameHash)) {
								
							}
							_internalTransitions [layer] = transitionNameHash;
						}
					}

				}
			}
		}
	}

	public class TransitionEventHandler
	{
		public event BaseAnimatorAccess.OnTransitionStartedHandler OnStarted;

		public TransitionEventHandler ()
		{
			Log.Temp ("Constructor");
			if (OnStarted == null) {}
		}
	}
	public class TransitionInfo
	{
		public int id;
		public int layer;
		public string layerName;
		public int sourceId;
		public int destId;
	}

}
