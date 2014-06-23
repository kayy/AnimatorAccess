// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public interface StateObserver
	{
		void Perform (Dictionary<int, StateInfo> stateInfos, LayerStatus [] statuses);
		/// <summary>
		/// Key string to build hash code so that every handler can be identified within the dictionary. 
		/// Naming convention is:
		/// ClassName[:LayerIndex[:StateHashId]]
		/// </summary>
		/// <returns>Key string as ClassName[:LayerIndex[:StateHashId]].</returns>
		string GetKeyString ();
	}

	public abstract class AbstractStateObserver : StateObserver
	{
		public abstract void Perform (Dictionary<int, StateInfo> stateInfos, LayerStatus[] statuses);

		public override int GetHashCode () {
			return GetKeyString ().GetHashCode ();
		}

		public virtual string GetKeyString () {
			return this.GetType ().Name;
		}

		protected StateInfo GetStateInfo (int stateId, Dictionary<int, StateInfo> stateInfos) {
			StateInfo info = null;
			if (stateInfos.ContainsKey (stateId)) {
				info = stateInfos [stateId];
			} else {
				Debug.LogWarning ("No state info found for state [" + stateId + "]!");
			}
			return info;
		}
	}

	public class AnyStateObserver : AbstractStateObserver
	{
		public event BaseAnimatorAccess.StateEventHandler OnChange;

		public override void Perform (Dictionary<int, StateInfo> stateInfos, LayerStatus [] statuses) {
			foreach (LayerStatus status in statuses) {
				if (status.State.HasChanged) {
					StateInfo info = GetStateInfo (status.State.Current, stateInfos);
					if (status != null) {
						OnChange (info, status);
					}
				}
			}
		}
	}

	public class SpecificStateObserver : AbstractStateObserver
	{
		public event BaseAnimatorAccess.StateEventHandler OnStay;
		public event BaseAnimatorAccess.StateEventHandler OnEnter;
		public event BaseAnimatorAccess.StateEventHandler OnExit;
		
		protected int layer = -1;
		protected int stateId = 0;
		
		public SpecificStateObserver (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}
		
		public override void Perform (Dictionary<int, StateInfo> stateInfos, LayerStatus [] statuses) {
			LayerStatus status = statuses [layer];
			if (status.State.HasChanged) {
				if (status.State.Current == stateId) {
					if (OnEnter != null) {
						StateInfo info = GetStateInfo (status.State.Current, stateInfos);
						if (info != null) {
							OnEnter (info, status);
						}
					}
				}
				if (status.State.Previous == stateId) {
					StateInfo info = GetStateInfo (status.State.Previous, stateInfos);
					if (OnExit != null) {
						OnExit (info, status);
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
	
	public class DynamicStateObserver : AnyStateObserver
	{
		System.Action<Dictionary<int, StateInfo>, LayerStatus []> performAction;

		public DynamicStateObserver (System.Action<Dictionary<int, StateInfo>, LayerStatus []> performAction) {
			this.performAction = performAction;
		}

		public override void Perform (Dictionary<int, StateInfo> stateInfos, LayerStatus[] statuses) {
			if (performAction != null) {
				performAction (stateInfos, statuses);
			} else {
				Debug.LogWarning ("No performAction supplied!");
			}
		}

		public override string GetKeyString () {
			return base.GetKeyString () + ":" + performAction.ToString ();
		}
	}
}