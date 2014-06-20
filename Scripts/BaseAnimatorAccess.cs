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
	public partial class BaseAnimatorAccess : MonoBehaviour 
	{
		/// <summary>
		/// Callback method for animator state changes.
		/// <param name="layer">Layer in which the state has changed.</param>
		/// <param name="newState">New state just entered.</param>
		/// <param name="previousState">Previous state.</param>
		/// </summary>
		public delegate void OnStateChangeHandler (LayerStateInfo info);

		public delegate void OnTransitionStartedHandler (TransitionInfo info);

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		public event OnStateChangeHandler OnStateChange;

		public event OnTransitionStartedHandler OnTransitionStarted;

		public Dictionary<int, string> stateDictionary = new Dictionary<int, string> ();
		
		public Dictionary<int, TransitionInfo> transitionInfos = new Dictionary<int, TransitionInfo> ();

		// TODO_kay: array? List<Handlers>
		Dictionary<int, TransitionEventHandler> transitionEventHandlers = new Dictionary<int, TransitionEventHandler> ();

		public LayerStateInfo [] _internalLayerInfo;
		int [] _internalTransitions;

		int _internalLayerCount = -1;

		protected virtual void Initialise (Animator animator) {
			_internalLayerCount = animator.layerCount;
			_internalLayerInfo = new LayerStateInfo [_internalLayerCount];
			for (int i = 0; i < _internalLayerCount; i++) {
				_internalLayerInfo [i].layer = i;
			}
			_internalTransitions = new int[animator.layerCount];
		}

		public TransitionEventHandler OnTransition (int src, int dest) {
			if (!transitionEventHandlers.ContainsKey (src)) {
				transitionEventHandlers [src] = new SingleTransitionEventHandler ();
			}
			return transitionEventHandlers [src];
		}

		public TransitionEventHandler OnTransitionFrom (int src) {
			if (!transitionEventHandlers.ContainsKey (src)) {
				transitionEventHandlers [src] = new FromStateTransitionEventHandler ();
			}
			return transitionEventHandlers [src];
		}

		public TransitionEventHandler OnAnyTransition () {
			if (!transitionEventHandlers.ContainsKey (0)) {
				transitionEventHandlers [0] = new AnyTransitionEventHandler ();
			}
			return transitionEventHandlers [0];
		}

		public string IdToName (int id) { 
			if (stateDictionary.ContainsKey (id)) {
				return (string)stateDictionary[id];
			}
			return "";
		}
		
		/// <summary>
		/// Checks for animator state changes if there are listeners registered in OnStateChange.
		/// </summary>
		/// <param name="animator">Animator instance for reading states of all layers.</param>
		public void CheckForAnimatorStateChanges (Animator animator) {
			if (_internalLayerCount < 0) {
				Initialise (animator);
			}
			for (int layer = 0; layer < _internalLayerCount; layer++) {
				_internalLayerInfo [layer].State.Current = animator.GetCurrentAnimatorStateInfo (layer).nameHash;
				if (animator.IsInTransition (layer)) {
					_internalLayerInfo [layer].Transition.Current = animator.GetAnimatorTransitionInfo (layer).nameHash;
				} else {
					_internalLayerInfo [layer].Transition.Current = 0;
				}
			}
			if (OnStateChange != null) {
				// TODO_kay: new state change handling
				for (int layer = 0; layer < _internalLayerCount; layer++) {
					if (_internalLayerInfo [layer].State.HasChanged) {
						OnStateChange (_internalLayerInfo [layer]);
					}
				}
			}
			if (transitionEventHandlers != null && transitionEventHandlers.Count > 0) {
				for (int layer = 0; layer < _internalLayerCount; layer++) {
					if (animator.IsInTransition (layer)) {
						AnimatorTransitionInfo animatorTransitionInfo = animator.GetAnimatorTransitionInfo (layer);
						int transitionNameHash = animatorTransitionInfo.nameHash;
						if (_internalTransitions [layer] != transitionNameHash) {
							TransitionInfo info = transitionInfos [transitionNameHash];
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

	public class LayerStateInfo
	{
		public int layer;
		public HistorisedProperty<int> State;
		public HistorisedProperty<int> Transition;
	}

	public class HistorisedProperty <T> where T : System.IComparable
	{
		T previous = default (T);
		public T Previous { get { return previous; } }
		
		T current = default (T);
		public T Current {
			get { return current; }
			set {
				previous = current;
				current = value;
			}
		}
		public bool HasChanged { get { return current.Equals (previous); } }
	}
	
}
