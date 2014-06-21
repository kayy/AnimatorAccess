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
		public delegate void OnStateChangeHandler (LayerStatus status);

		public delegate void OnTransitionStartedHandler (TransitionInfo info);

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		[System.ObsoleteAttribute ("Use AnyState ().OnChange instead or State (stateId).OnChange for a single state.", false)]
		public event OnStateChangeHandler OnStateChange {
			add { AnyState ().OnChange += value;}
			remove { AnyState ().OnChange -= value; }
		}

		public Dictionary<int, StateInfo> stateDictionary = new Dictionary<int, StateInfo> ();

		public Dictionary<int, StateEventHandler> stateEventHandlers = new Dictionary<int, StateEventHandler> ();

		public Dictionary<int, TransitionInfo> transitionInfos = new Dictionary<int, TransitionInfo> ();

		Dictionary<int, TransitionEventHandler> transitionEventHandlers = new Dictionary<int, TransitionEventHandler> ();

		public LayerStatus [] _internalLayerStatus;

		int _internalLayerCount = -1;

		protected virtual void Initialise (Animator animator) {
			_internalLayerCount = animator.layerCount;
			_internalLayerStatus = new LayerStatus [_internalLayerCount];
			for (int i = 0; i < _internalLayerCount; i++) {
				_internalLayerStatus [i] = new LayerStatus (i, 0, 0);
			}
		}

		public SpecificStateChangeEventHandler State (int nameHash) {
			if (stateDictionary.ContainsKey (nameHash)) {
				StateInfo info = stateDictionary [nameHash];
				// reuse handler if possible to maximise performance in FixedUdpate; 
				// drawback: we have to create a handler first to get the hash ID
				SpecificStateChangeEventHandler handler = new SpecificStateChangeEventHandler (info.layer, nameHash);
				int id = handler.GetHashCode ();
				if (!stateEventHandlers.ContainsKey (id)) {
					stateEventHandlers [id] = handler;
				} else {
				}
				return (SpecificStateChangeEventHandler)stateEventHandlers [id];
			} else {
				Debug.LogWarning ("There seem to be no animator state with nameHash [" + nameHash + "]. Maybe you need to update the corresonding AnimatorAccess component.");
			}
			return null;
		}

		public AnyStateChangeEventHandler AnyState () {
			AnyStateChangeEventHandler handler = new AnyStateChangeEventHandler ();
			int id = handler.GetHashCode ();
			if (!stateEventHandlers.ContainsKey (id)) {
				Log.Temp ("new ANY Handler");
				stateEventHandlers [id] = handler;
			}
			return (AnyStateChangeEventHandler)stateEventHandlers [id];
		}
		
		public SpecificTransitionEventHandler Transition (int source, int dest) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in transitionInfos.Values) {
				if (ti.sourceId == source && ti.destId == dest) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				SpecificTransitionEventHandler handler = new SpecificTransitionEventHandler (info.layer, info.id);
				int id = handler.GetHashCode ();
				if (!transitionEventHandlers.ContainsKey (id)) {
					Log.Temp ("new specific Handler");
					transitionEventHandlers [id] = handler;
				} else {
					Log.Temp ("Found existing specific handler");
				}
				return (SpecificTransitionEventHandler)transitionEventHandlers [id];
			} else {
				Debug.LogWarning ("There seem to be no transition from state [" + source + "] to [" + dest + "]. Maybe you need to update the corresonding AnimatorAccess component.");
				return null;
			}
		}

		public FromStateTransitionEventHandler TransitionFrom (int source) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in transitionInfos.Values) {
				if (ti.sourceId == source) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				FromStateTransitionEventHandler handler = new FromStateTransitionEventHandler (info.layer, info.id);
				int id = handler.GetHashCode ();
				if (!transitionEventHandlers.ContainsKey (id)) {
					Log.Temp ("new FromStateTransition Handler");
					transitionEventHandlers [id] = handler;
				} else { 
					Log.Temp ("Found existing FromStateTransition handler");
				}
				return (FromStateTransitionEventHandler)transitionEventHandlers [id];
			} else {
				Debug.LogWarning ("There seem to be no transitions from state [" + source + "]. Maybe you need to update the corresonding AnimatorAccess component.");
				return null;
			}
		}

		public AnyTransitionEventHandler AnyTransition () {
			AnyTransitionEventHandler handler = new AnyTransitionEventHandler ();
			int id = handler.GetHashCode ();
			if (!transitionEventHandlers.ContainsKey (id)) {
				transitionEventHandlers [0] = handler;
			}
			return (AnyTransitionEventHandler)transitionEventHandlers [0];
		}

		public string IdToName (int id) { 
			if (stateDictionary.ContainsKey (id)) {
				return stateDictionary [id].stateName;
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
				_internalLayerStatus [layer].State.Current = animator.GetCurrentAnimatorStateInfo (layer).nameHash;
				if (animator.IsInTransition (layer)) {
					_internalLayerStatus [layer].Transition.Current = animator.GetAnimatorTransitionInfo (layer).nameHash;
				} else {
					_internalLayerStatus [layer].Transition.Current = 0;
				}
			}
			if (stateEventHandlers != null && stateEventHandlers.Count > 0) {
				foreach (StateEventHandler handler in stateEventHandlers.Values) {
					handler.Perform (_internalLayerStatus);
				}
			}
			if (transitionEventHandlers != null && transitionEventHandlers.Count > 0) {
				foreach (TransitionEventHandler handler in transitionEventHandlers.Values) {
					handler.Perform (animator, transitionInfos);
				}
			}
		}
	}

	public class LayerStatus
	{
		public int layer;
		public HistorisedProperty<int> State = new HistorisedProperty<int> ();
		public HistorisedProperty<int> Transition = new HistorisedProperty<int> ();

		public LayerStatus (int layer, int state, int transition) {
			this.layer = layer;
			State.Current = state;
			Transition.Current = transition;
		}
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
		public bool HasChanged { get { return !current.Equals (previous); } }
	}
	
}
