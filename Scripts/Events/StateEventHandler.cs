// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public interface StateEventHandler
	{
		void Perform (LayerStatus [] infos);
		/// <summary>
		/// Key string to build hash code so that every handler can be identified within the dictionary. 
		/// Naming convention is:
		/// ClassName[:LayerIndex[:StateHashId]]
		/// </summary>
		/// <returns>Key string as ClassName[:LayerIndex[:StateHashId]].</returns>
		string GetKeyString ();
	}

	public abstract class AbstractStateEventHandler : StateEventHandler
	{
		public abstract void Perform (LayerStatus[] infos);

		public override int GetHashCode () {
			return GetKeyString ().GetHashCode ();
		}

		public virtual string GetKeyString () {
			return this.GetType ().Name;
		}
	}

	public class AnyStateChangeEventHandler : AbstractStateEventHandler
	{
		public event BaseAnimatorAccess.OnStateChangeHandler OnChange;

		public override void Perform (LayerStatus [] infos) {
			foreach (LayerStatus info in infos) {
				if (info.State.HasChanged) {
					OnChange (info);
				}
			}
		}
	}

	public class SpecificStateChangeEventHandler : AbstractStateEventHandler
	{
		public event BaseAnimatorAccess.OnStateChangeHandler OnChange;
		public event BaseAnimatorAccess.OnStateChangeHandler OnEnter;
		public event BaseAnimatorAccess.OnStateChangeHandler OnExit;
		
		protected int layer = -1;
		protected int stateId = 0;
		
		public SpecificStateChangeEventHandler (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}
		
		public override void Perform (LayerStatus [] infos) {
			LayerStatus info = infos [layer];
			if (info.State.HasChanged) {
				if (info.State.Current == stateId) {
					if (OnEnter != null) {
						OnEnter (info);
					}
					if (OnChange != null) {
						OnChange (info);
					}
				}
				if (info.State.Previous == stateId) {
					if (OnChange != null) {
						OnChange (info);
					}
					if (OnExit != null) {
						OnExit (info);
					}
				}
			}
		}

		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + stateId;
		}
	}
	
	public class DynamicStateEventHandler : AnyStateChangeEventHandler
	{
		System.Action<LayerStatus []> performAction;

		public DynamicStateEventHandler (System.Action<LayerStatus []> performAction) {
			this.performAction = performAction;
		}

		public override void Perform (LayerStatus[] infos) {
			if (performAction != null) {
				performAction (infos);
			} else {
				Debug.LogWarning ("No performAction supplied!");
			}
		}

		public override string GetKeyString () {
			return base.GetKeyString () + ":" + performAction.ToString ();
		}
	}
}