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
	public class SpecificStateHandler : AbstractStateHandler
	{
		/// <summary>
		/// Occurs as long as this state is active. Contrary to OnStay subscribers will get notified too when this state
		/// is just entered.
		/// </summary>
		public event BaseAnimatorAccess.StateEventHandler OnActive;
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
		/// not raised when this state is just entered or has just left (see also OnActive).
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
		
		public SpecificStateHandler (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}
		
		public override void Perform (LayerStatus [] statuses, Dictionary<int, StateInfo> stateInfos) {
			LayerStatus status = statuses [layer];
			if (status.State.Current == stateId) {
				// seems awkward but null check first costs almost no performance, but a dictionary lookup does
				if (OnActive != null || OnEnter != null || OnStay != null) {
					StateInfo currentInfo = GetStateInfo (status.State.Current, stateInfos);
					if (currentInfo != null) {
						if (status.State.HasChanged) {
							if (OnEnter != null) {
								// fire once after a state change
								OnEnter (currentInfo, status);
							}
						} else if (OnStay != null) {
							// OnStay starts firing the 2nd time we are in this state
							OnStay (currentInfo, status);
						}
						if (OnActive != null) {
							// fire always
							OnActive (currentInfo, status);
						}
					}
				}
			} else if (status.State.Previous == stateId && status.State.HasChanged) {
				StateInfo previousInfo = GetStateInfo (status.State.Previous, stateInfos);
				if (OnExit != null) {
					OnExit (previousInfo, status);
				}
			}
		}
		
		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + stateId;
		}
	}
}
