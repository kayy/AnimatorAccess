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
		public delegate void StateEventHandler (StateInfo info, LayerStatus status);

		public delegate void TransitionEventHandler (TransitionInfo info, LayerStatus status);

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		[System.ObsoleteAttribute ("Use AnyState ().OnChange instead or State (stateId).OnChange for a single state.", false)]
		public event StateEventHandler OnStateChange {
			add { AnyState ().OnChange += value;}
			remove { AnyState ().OnChange -= value; }
		}

		public Dictionary<int, StateInfo> StateInfos = new Dictionary<int, StateInfo> ();

		public Dictionary<int, StateObserver> StateObservers = new Dictionary<int, StateObserver> ();

		public Dictionary<int, TransitionInfo> TransitionInfos = new Dictionary<int, TransitionInfo> ();

		public Dictionary<int, TransitionObserver> TransitionObservers = new Dictionary<int, TransitionObserver> ();

		public LayerStatus [] LayerStatuses;

		int _internalLayerCount = -1;

		protected virtual void Initialise (Animator animator) {
			_internalLayerCount = animator.layerCount;
			LayerStatuses = new LayerStatus [_internalLayerCount];
			for (int i = 0; i < _internalLayerCount; i++) {
				LayerStatuses [i] = new LayerStatus (i, 0, 0);
			}
		}

		public SpecificStateObserver State (int nameHash) {
			if (StateInfos.ContainsKey (nameHash)) {
				StateInfo info = StateInfos [nameHash];
				// reuse handler if possible to maximise performance in FixedUdpate; 
				// drawback: we have to create a handler first to get the hash ID
				SpecificStateObserver handler = new SpecificStateObserver (info.layer, nameHash);
				int id = handler.GetHashCode ();
				if (!StateObservers.ContainsKey (id)) {
					StateObservers [id] = handler;
				} else {
				}
				return (SpecificStateObserver)StateObservers [id];
			} else {
				Debug.LogWarning ("There seem to be no animator state with nameHash [" + nameHash + "]. Maybe you need to update the corresonding AnimatorAccess component.");
			}
			return null;
		}

		public AnyStateObserver AnyState () {
			AnyStateObserver handler = new AnyStateObserver ();
			int id = handler.GetHashCode ();
			if (!StateObservers.ContainsKey (id)) {
				Log.Temp ("new ANY Handler");
				StateObservers [id] = handler;
			}
			return (AnyStateObserver)StateObservers [id];
		}
		
		public SpecificTransitionObserver Transition (int source, int dest) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in TransitionInfos.Values) {
				if (ti.sourceId == source && ti.destId == dest) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				SpecificTransitionObserver handler = new SpecificTransitionObserver (info.layer, info.id);
				int id = handler.GetHashCode ();
				if (!TransitionObservers.ContainsKey (id)) {
					Log.Temp ("new specific Handler");
					TransitionObservers [id] = handler;
				} else {
					Log.Temp ("Found existing specific handler");
				}
				return (SpecificTransitionObserver)TransitionObservers [id];
			} else {
				Debug.LogWarning ("There seem to be no transition from state [" + source + "] to [" + dest + "]. Maybe you need to update the corresonding AnimatorAccess component.");
				return null;
			}
		}

		public FromStateTransitionObserver TransitionFrom (int source) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in TransitionInfos.Values) {
				if (ti.sourceId == source) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				FromStateTransitionObserver handler = new FromStateTransitionObserver (info.layer, info.id);
				int id = handler.GetHashCode ();
				if (!TransitionObservers.ContainsKey (id)) {
					Log.Temp ("new FromStateTransition Handler");
					TransitionObservers [id] = handler;
				} else { 
					Log.Temp ("Found existing FromStateTransition handler");
				}
				return (FromStateTransitionObserver)TransitionObservers [id];
			} else {
				Debug.LogWarning ("There seem to be no transitions from state [" + source + "]. Maybe you need to update the corresonding AnimatorAccess component.");
				return null;
			}
		}

		public AnyTransitionObserver AnyTransition () {
			AnyTransitionObserver handler = new AnyTransitionObserver ();
			int id = handler.GetHashCode ();
			if (!TransitionObservers.ContainsKey (id)) {
				TransitionObservers [0] = handler;
			}
			return (AnyTransitionObserver)TransitionObservers [0];
		}

		public string IdToName (int id) { 
			if (StateInfos.ContainsKey (id)) {
				return StateInfos [id].stateName;
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
				LayerStatuses [layer].State.Current = animator.GetCurrentAnimatorStateInfo (layer).nameHash;
				if (animator.IsInTransition (layer)) {
					LayerStatuses [layer].Transition.Current = animator.GetAnimatorTransitionInfo (layer).nameHash;
				} else {
					LayerStatuses [layer].Transition.Current = 0;
				}
			}
			if (StateObservers != null && StateObservers.Count > 0) {
				foreach (StateObserver handler in StateObservers.Values) {
					handler.Perform (StateInfos, LayerStatuses);
				}
			}
			if (TransitionObservers != null && TransitionObservers.Count > 0) {
				foreach (TransitionObserver handler in TransitionObservers.Values) {
					handler.Perform (LayerStatuses, TransitionInfos);
				}
			}
		}
	}

}
