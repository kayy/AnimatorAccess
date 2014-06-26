// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Base class to handle transitions related to a specific state like 'from' or 'to' a state. Listeners can 
	/// subscribe to OnActive, OnStarted or OnFinished.
	/// </summary>
	public abstract class AbstractStateTransitionHandler : AbstractTransitionHandler
	{
		/// <summary>
		/// Occurs as long as a transition related to the specified state is ongoing.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnActive;
		/// <summary>
		/// Occurs when a new transition to the specified state is started.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;
		/// <summary>
		/// Occurs when the ongoing transition to the specified state has finished.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnFinished;
		
		protected int layer = -1;
		protected int stateId = 0;
		
		protected AbstractStateTransitionHandler (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}

		protected abstract int GetObservedState (TransitionInfo info);

		public override void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
			LayerStatus status = statuses [layer];
			if (status.Transition.HasChanged) {
				if (OnStarted != null) {
					TransitionInfo currentInfo = GetTransitionInfo (status.Transition.Current, transitionInfos);
					if (currentInfo != null && GetObservedState (currentInfo) == stateId) {
						OnStarted (currentInfo, status);
					}
				}
				if (OnFinished != null) {
					TransitionInfo previousInfo = GetTransitionInfo (status.Transition.Previous, transitionInfos);
					if (previousInfo != null && GetObservedState (previousInfo) == stateId) {
						OnFinished (previousInfo, status);
					}
				}
			}
			if (OnActive != null) {
				TransitionInfo currentInfo = GetTransitionInfo (status.Transition.Current, transitionInfos);
				if (currentInfo != null && GetObservedState (currentInfo) == stateId) {
					OnActive (currentInfo, status);
				}
			}
		}
		
		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + stateId;
		}
	}
}

