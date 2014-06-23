// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Observes one specific transition i.e. FROM -> TO.
	/// </summary>
	public class SpecificTransitionObserver : AbstractTransitionObserver
	{
		/// <summary>
		/// Occurs when the specified transition is started.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;
		/// <summary>
		/// Occurs when the specified transition finished.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnFinished;
		/// <summary>
		/// Occurs as long as the specified transition is ongoing. Note that the first occurumce is omitted as it is 
		/// reported by OnStarted.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnStay;

		/// <summary>
		/// The layer where the specified transition is residing.
		/// </summary>
		protected int layer = -1;
		/// <summary>
		/// The transition name hash.
		/// </summary>
		protected int transitionId = 0;
		
		public SpecificTransitionObserver (int layer, int transitionId)
		{
			this.layer = layer;
			this.transitionId = transitionId;
		}
		
		public override void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos)
		{
			LayerStatus status = statuses [layer];
			int currentTransitionId = status.Transition.Current;
			if (status.Transition.HasChanged) {
				if (currentTransitionId == transitionId && OnStarted != null) {
					TransitionInfo currentInfo = GetTransitionInfo (currentTransitionId, transitionInfos);
					if (currentInfo != null) {
						OnStarted (currentInfo, status);
					}
				}
				int previousTransitionId = status.Transition.Previous;
				if (OnFinished != null && previousTransitionId == transitionId) {
					TransitionInfo previousInfo = GetTransitionInfo (previousTransitionId, transitionInfos);
					if (previousInfo != null) {
						OnFinished (previousInfo, status);
					}
				}
			} else if (OnStay != null && currentTransitionId == transitionId) {
				// OnStay starts firing one cycle after OnStarted
				TransitionInfo info = GetTransitionInfo (currentTransitionId, transitionInfos);
				OnStay (info, status);
			}
		}
		
		public override string GetKeyString ()
		{
			return base.GetKeyString () + ":" + layer + ":" + transitionId;
		}
	}
}

