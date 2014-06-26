// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Notifies all listeners whenever a transition to the specified state occurs. Listeners can subscribe to 
	/// OnStarted or OnFinished.
	/// </summary>
	public class ToStateTransitionHandler : AbstractStateTransitionHandler
	{
		public ToStateTransitionHandler (int layer, int stateId) : base (layer, stateId) {
		}

		protected override int GetObservedState (TransitionInfo info) {
			return info.DestId;
		}
	}
}

