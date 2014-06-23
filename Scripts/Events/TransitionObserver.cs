// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public interface TransitionObserver
	{
		void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos);

		/// <summary>
		/// Key string to build hash code so that every handler can be identified within the dictionary. 
		/// Naming convention is:
		/// ClassName[:LayerIndex[:StateHashId]]
		/// </summary>
		/// <returns>Key string as ClassName[:LayerIndex[:StateHashId]].</returns>
		string GetKeyString ();
	}

	public abstract class AbstractTransitionObserver : TransitionObserver
	{
		protected AbstractTransitionObserver () {
		}

		public virtual void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
		}

		public override int GetHashCode () {
			return GetKeyString ().GetHashCode ();
		}
		
		public virtual string GetKeyString () {
			return this.GetType ().Name;
		}

		protected TransitionInfo GetTransitionInfo (int transitionId, Dictionary<int, TransitionInfo> transitionInfos) {
			TransitionInfo info = null;
			if (transitionId != 0) {
				if (transitionInfos.ContainsKey (transitionId)) {
					info = transitionInfos [transitionId];
				} else {
					Debug.LogWarning ("No transition info found for transition [" + transitionId + "]!");
				}
			}
			return info;
		}
	}

	public class AnyTransitionObserver : AbstractTransitionObserver
	{
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;

		public event BaseAnimatorAccess.TransitionEventHandler OnFinished;
		
		public override void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
			if (OnStarted != null || OnFinished != null) {
				foreach (LayerStatus status in statuses) {
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
	}
	
	public class SpecificTransitionObserver : AbstractTransitionObserver
	{
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;
		
		public event BaseAnimatorAccess.TransitionEventHandler OnFinished;

		public event BaseAnimatorAccess.TransitionEventHandler OnStay;
		
		protected int layer = -1;
		protected int transitionId = 0;

		public SpecificTransitionObserver (int layer, int transitionId) {
			this.layer = layer;
			this.transitionId = transitionId;
		}

		public override void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
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

		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + transitionId;
		}
	}
	
	public class FromStateTransitionObserver : AbstractTransitionObserver
	{
		public event BaseAnimatorAccess.TransitionEventHandler OnStarted;
		
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
					if (currentInfo != null && currentInfo.sourceId == stateId) {
						OnStarted (currentInfo, status);
					}
				}
				if (OnFinished != null) {
					TransitionInfo previousInfo = GetTransitionInfo (status.Transition.Previous, transitionInfos);
					if (previousInfo != null && previousInfo.sourceId == stateId) {
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

