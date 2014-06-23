// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Observes any state change of the specified layer or of all layers if layer == -1. OnChange event with the state 
	/// just enterd is raised if there is a state change. If you need more fine grained notifications use 
	/// SpecificStateObserver.
	/// </summary>
	public class AnyStateObserver : AbstractStateObserver
	{
		/// <summary>
		/// Occurs when Animator state has changed.
		/// </summary>
		public event BaseAnimatorAccess.StateEventHandler OnChange;
		
		/// <summary>
		/// Layer to observe or -1 for all layers.
		/// </summary>
		protected int layer;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnimatorAccess.AnyStateObserver"/> class.
		/// </summary>
		/// <param name="layer">Layer index or -1 to check all layers.</param>
		public AnyStateObserver (int layer = -1) {
			this.layer = layer;
		}

		public override void Perform (LayerStatus[] statuses, Dictionary<int, StateInfo> stateInfos) {
			if (layer == -1) {
				foreach (LayerStatus status in statuses) {
					CheckLayerStatus (status, stateInfos);
				}
			} else {
				CheckLayerStatus (statuses [layer], stateInfos);
			}
		}

		void CheckLayerStatus (LayerStatus status, Dictionary<int, StateInfo> stateInfos) {
			if (status.State.HasChanged) {
				StateInfo info = GetStateInfo (status.State.Current, stateInfos);
				if (status != null) {
					OnChange (info, status);
				}
			}
		}
	}
}

