// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public interface StateEventHandler
	{
		event BaseAnimatorAccess.OnStateChangeHandler OnChange;
		
		void Perform (LayerStateInfo [] infos);
	}

	public class AnyStateEventHandler : StateEventHandler
	{
		public event BaseAnimatorAccess.OnStateChangeHandler OnChange;

		public virtual void Perform (LayerStateInfo [] infos) {
			foreach (LayerStateInfo info in infos) {
				if (info.State.HasChanged) {
					OnChange (info);
				}
			}
		}

	}

	public class DynamicStateEventHandler : AnyStateEventHandler
	{
		System.Action<LayerStateInfo []> performAction;

		public DynamicStateEventHandler (System.Action<LayerStateInfo []> performAction) {
			this.performAction = performAction;
		}

		public override void Perform (LayerStateInfo[] infos)
		{
			if (performAction != null) {
				performAction (infos);
			} else {
				Debug.LogWarning ("No performAction supplied!");
			}
		}
	}
}