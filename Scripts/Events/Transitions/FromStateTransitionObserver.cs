// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;


namespace AnimatorAccess
{
	/// <summary>
	/// Notifies all listeners whenever a trasition from the specified state is started or finished.
	/// </summary>
	public class FromStateTransitionObserver : AbstractTransitionObserver
	{
		/// <summary>
		/// Occurs when a new transition from the specified state is started.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;
		/// <summary>
		/// Occurs when the ongoing transition from the specified state just has finished.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnFinished;
		
		protected int layer = -1;
		protected int stateId = 0;
		
		public FromStateTransitionObserver (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}
		
		public override void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
			LayerStatus status = statuses [layer];
			if (status.Transition.HasChanged) {
				if (OnStarted != null) {
					TransitionInfo currentInfo = GetTransitionInfo (status.Transition.Current, transitionInfos);
					if (currentInfo != null && currentInfo.SourceId == stateId) {
						OnStarted (currentInfo, status);
					}
				}
				if (OnFinished != null) {
					TransitionInfo previousInfo = GetTransitionInfo (status.Transition.Previous, transitionInfos);
					if (previousInfo != null && previousInfo.SourceId == stateId) {
						OnFinished (previousInfo, status);
					}
				}
			}
		}
		
		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + stateId;
		}
	}
}

