// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Observes any transitions of the specified layer or of all layers if layer == -1. See FromStateTransitionObserver
	/// and SpecificTransitionObserver for more specific event notifications.
	/// </summary>
	public class AnyTransitionObserver : AbstractTransitionObserver
	{
		/// <summary>
		/// Occurs when a new transition is started.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;
		/// <summary>
		/// Occurs when a transition is finished.
		/// </summary>
		public event BaseAnimatorAccess.TransitionEventHandler OnFinished;
		
		/// <summary>
		/// Layer to observe or -1 for all layers.
		/// </summary>
		protected int layer;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnimatorAccess.AnyTransitionObserver"/> class.
		/// </summary>
		/// <param name="layer">Layer index or -1 to observe all layers.</param>
		public AnyTransitionObserver (int layer = -1) {
			this.layer = layer;
		}
		
		public override void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
			if (layer == -1) {
				foreach (LayerStatus status in statuses) {
					CheckTransitionStatus (status, transitionInfos);
				}
			} else {
				CheckTransitionStatus (statuses [layer], transitionInfos);
			}
		}
		
		void CheckTransitionStatus (LayerStatus status, Dictionary<int, TransitionInfo> transitionInfos) {
			if (status.Transition.HasChanged) {
				int currentTransitionId = status.Transition.Current;
				if (currentTransitionId != 0 && OnStarted != null) {
					TransitionInfo currentInfo = GetTransitionInfo (currentTransitionId, transitionInfos);
					if (currentInfo != null) {
						OnStarted (currentInfo, status);
					}
				}
				int previousTransitionId = status.Transition.Previous;
				if (previousTransitionId != 0 && OnFinished != null) {
					TransitionInfo previousInfo = GetTransitionInfo (previousTransitionId, transitionInfos);
					if (previousInfo != null) {
						OnFinished (previousInfo, status);
					}
				}
			}
		}
	}
	

}