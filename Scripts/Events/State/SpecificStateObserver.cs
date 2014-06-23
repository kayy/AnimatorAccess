// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Notifies its listeners whenever anything related to the specified state changes. Possible event notifications 
	/// are OnEnter, OnExit and OnStay.
	/// </summary>
	public class SpecificStateObserver : AbstractStateObserver
	{
		/// <summary>
		/// Occurs once when this state is entered.
		/// </summary>
		public event BaseAnimatorAccess.StateEventHandler OnEnter;
		/// <summary>
		/// Occurs once when this state is left. info contains this i.e. just left state. To get the current state use
		/// status.Current.
		/// </summary>
		public event BaseAnimatorAccess.StateEventHandler OnExit;
		/// <summary>
		/// Occurs on every cycle as long as the state machine is remaining in this state. Note that this event is 
		/// not raised when this state is just entered or has just left.
		/// </summary>
		public event BaseAnimatorAccess.StateEventHandler OnStay;

		/// <summary>
		/// Layer containing stateId.
		/// </summary>
		protected int layer = -1;
		/// <summary>
		/// Animator state hash to observe.
		/// </summary>
		protected int stateId = 0;
		
		public SpecificStateObserver (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}
		
		public override void Perform (LayerStatus [] statuses, Dictionary<int, StateInfo> stateInfos) {
			LayerStatus status = statuses [layer];
			if (status.State.HasChanged) {
				if (status.State.Current == stateId) {
					if (OnEnter != null) {
						StateInfo currentInfo = GetStateInfo (status.State.Current, stateInfos);
						if (currentInfo != null) {
							OnEnter (currentInfo, status);
						}
					}
				}
				if (status.State.Previous == stateId) {
					StateInfo previousInfo = GetStateInfo (status.State.Previous, stateInfos);
					if (OnExit != null) {
						OnExit (previousInfo, status);
					}
				}
			} else if (OnStay != null && status.State.Current == stateId) {
				// OnStay starts firing one cycle after OnStarted
				StateInfo info = GetStateInfo (status.State.Current, stateInfos);
				if (info != null) {
					OnStay (info, status);
				}
			}
		}
		
		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + stateId;
		}
	}
}
