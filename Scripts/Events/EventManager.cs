// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public class EventManager
	{
		/// <summary>
		/// Every layer's current status, updated in CheckForAnimatorStateChanges.
		/// </summary>
		public LayerStatus [] LayerStatuses;
		
		public Dictionary<int, StateInfo> StateInfos = new Dictionary<int, StateInfo> ();
		
		public Dictionary<int, StateObserver> StateObservers = new Dictionary<int, StateObserver> ();
		
		public Dictionary<int, TransitionInfo> TransitionInfos = new Dictionary<int, TransitionInfo> ();
		
		public Dictionary<int, TransitionObserver> TransitionObservers = new Dictionary<int, TransitionObserver> ();

		/// <summary>
		/// Retrieves a reference to observe a single state.
		/// </summary>
		/// <param name="nameHash">Name hash of the state to observe.</param>
		public SpecificStateObserver State (int nameHash) {
			if (StateInfos.ContainsKey (nameHash)) {
				StateInfo info = StateInfos [nameHash];
				// reuse handler if possible to maximise performance in FixedUdpate; 
				// drawback: we have to create a handler first to get the hash ID
				SpecificStateObserver handler = new SpecificStateObserver (info.Layer, nameHash);
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
		
		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="layer">If set i.e. != -1, only the speicfied layer will be observed.</param>
		public AnyStateObserver AnyState (int layer = -1) {
			AnyStateObserver handler = new AnyStateObserver (layer);
			int id = handler.GetHashCode ();
			if (!StateObservers.ContainsKey (id)) {
				StateObservers [id] = handler;
			}
			return (AnyStateObserver)StateObservers [id];
		}
		
		public SpecificTransitionObserver Transition (int source, int dest) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in TransitionInfos.Values) {
				if (ti.SourceId == source && ti.DestId == dest) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				SpecificTransitionObserver handler = new SpecificTransitionObserver (info.Layer, info.Id);
				int id = handler.GetHashCode ();
				if (!TransitionObservers.ContainsKey (id)) {
					TransitionObservers [id] = handler;
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
				if (ti.SourceId == source) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				FromStateTransitionObserver handler = new FromStateTransitionObserver (info.Layer, info.Id);
				int id = handler.GetHashCode ();
				if (!TransitionObservers.ContainsKey (id)) {
					TransitionObservers [id] = handler;
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
				return StateInfos [id].Name;
			}
			return "";
		}

		int _internalLayerCount = -1;
		
		public void Initialise (Animator animator, BaseAnimatorAccess animatorAccess) {
			_internalLayerCount = animator.layerCount;
			LayerStatuses = new LayerStatus [_internalLayerCount];
			for (int i = 0; i < _internalLayerCount; i++) {
				LayerStatuses [i] = new LayerStatus (i, 0, 0);
			}
			animatorAccess.InitialiseEventManager ();
		}

		/// <summary>
		/// Checks for animator state changes if there are listeners registered in OnStateChange.
		/// </summary>
		/// <param name="animator">Animator instance for reading states of all layers.</param>
		public void CheckForAnimatorStateChanges (Animator animator) {
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
					handler.Perform (LayerStatuses, StateInfos);
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
