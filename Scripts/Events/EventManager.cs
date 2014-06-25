// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Handles state and transition event management for BaseAnimatorAccess. See comments there for more information.
	/// All members are intentionally declared public to support extending BaseAccessAnimator with new handlers.
	/// </summary>
	public class EventManager
	{
		public int LayerCount = -1;
		
		public LayerStatus [] LayerStatuses;
		
		public Dictionary<int, StateInfo> StateInfos = new Dictionary<int, StateInfo> ();
		
		public Dictionary<int, StateHandler> StateHandlers = new Dictionary<int, StateHandler> ();
		
		public Dictionary<int, TransitionInfo> TransitionInfos = new Dictionary<int, TransitionInfo> ();
		
		public Dictionary<int, TransitionHandler> TransitionHandlers = new Dictionary<int, TransitionHandler> ();

		public void Initialise (Animator animator, BaseAnimatorAccess animatorAccess) {
			LayerCount = animator.layerCount;
			LayerStatuses = new LayerStatus [LayerCount];
			for (int i = 0; i < LayerCount; i++) {
				LayerStatuses [i] = new LayerStatus (i, 0, 0);
			}
			// callback to overriden method in generated class to initialise state and transition infos
			animatorAccess.InitialiseEventManager ();
		}
		
#region State Event Handling
		public AnyStateHandler AnyState (int layer = -1) {
			if (layer > LayerCount) {
				Debug.LogWarning ("The specified layer " + layer + " exceeds layer count (" + LayerCount + ")! Seems like the AnimatorAccess component needs to be updated.");
			}
			AnyStateHandler handler = new AnyStateHandler (layer);
			int id = handler.GetHashCode ();
			if (!StateHandlers.ContainsKey (id)) {
				StateHandlers [id] = handler;
			}
			return (AnyStateHandler)StateHandlers [id];
		}

		public SpecificStateHandler State (int nameHash) {
			if (StateInfos.ContainsKey (nameHash)) {
				StateInfo info = StateInfos [nameHash];
				// reuse handler if possible to maximise performance in FixedUdpate; 
				// drawback: we have to create a handler first to get the hash ID
				SpecificStateHandler handler = new SpecificStateHandler (info.Layer, nameHash);
				int id = handler.GetHashCode ();
				if (!StateHandlers.ContainsKey (id)) {
					StateHandlers [id] = handler;
				} else {
				}
				return (SpecificStateHandler)StateHandlers [id];
			} else {
				Debug.LogWarning ("There seem to be no animator state with nameHash [" + nameHash + "]. Maybe you need to update the corresonding AnimatorAccess component.");
			}
			return null;
		}
#endregion
		
#region Transition Event Handling
		public AnyTransitionHandler AnyTransition (int layer = -1) {
			if (layer > LayerCount) {
				Debug.LogWarning ("The specified layer " + layer + " exceeds layer count (" + LayerCount + ")! Seems like the AnimatorAccess component needs to be updated.");
			}
			AnyTransitionHandler handler = new AnyTransitionHandler (layer);
			int id = handler.GetHashCode ();
			if (!TransitionHandlers.ContainsKey (id)) {
				TransitionHandlers [0] = handler;
			}
			return (AnyTransitionHandler)TransitionHandlers [0];
		}

		public FromStateTransitionHandler TransitionFrom (int source) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in TransitionInfos.Values) {
				if (ti.SourceId == source) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				FromStateTransitionHandler handler = new FromStateTransitionHandler (info.Layer, info.Id);
				int id = handler.GetHashCode ();
				if (!TransitionHandlers.ContainsKey (id)) {
					TransitionHandlers [id] = handler;
				}
				return (FromStateTransitionHandler)TransitionHandlers [id];
			} else {
				Debug.LogWarning ("There seem to be no transitions from state [" + source + "]. Maybe you need to update the corresonding AnimatorAccess component.");
				return null;
			}
		}

		public SpecificTransitionHandler Transition (int source, int dest) {
			TransitionInfo info = null;
			foreach (TransitionInfo ti in TransitionInfos.Values) {
				if (ti.SourceId == source && ti.DestId == dest) {
					info = ti;
					break;
				}
			}
			if (info != null) {
				SpecificTransitionHandler handler = new SpecificTransitionHandler (info.Layer, info.Id);
				int id = handler.GetHashCode ();
				if (!TransitionHandlers.ContainsKey (id)) {
					TransitionHandlers [id] = handler;
				}
				return (SpecificTransitionHandler)TransitionHandlers [id];
			} else {
				Debug.LogWarning ("There seem to be no transition from state [" + source + "] to [" + dest + "]. Maybe you need to update the corresonding AnimatorAccess component.");
				return null;
			}
		}
#endregion

		public string GetStateName (int id) { 
			if (StateInfos.ContainsKey (id)) {
				return StateInfos [id].Name;
			}
			return "";
		}
		
		public string GetTransitionName (int id) { 
			if (TransitionInfos.ContainsKey (id)) {
				return TransitionInfos [id].Name;
			}
			return "";
		}
		
		public void CheckForAnimatorStateChanges (Animator animator) {
			for (int layer = 0; layer < LayerCount; layer++) {
				LayerStatuses [layer].State.Current = animator.GetCurrentAnimatorStateInfo (layer).nameHash;
				if (animator.IsInTransition (layer)) {
					LayerStatuses [layer].Transition.Current = animator.GetAnimatorTransitionInfo (layer).nameHash;
				} else {
					LayerStatuses [layer].Transition.Current = 0;
				}
			}
			if (StateHandlers != null && StateHandlers.Count > 0) {
				foreach (StateHandler handler in StateHandlers.Values) {
					handler.Perform (LayerStatuses, StateInfos);
				}
			}
			if (TransitionHandlers != null && TransitionHandlers.Count > 0) {
				foreach (TransitionHandler handler in TransitionHandlers.Values) {
					handler.Perform (LayerStatuses, TransitionInfos);
				}
			}
		}
	}
}
