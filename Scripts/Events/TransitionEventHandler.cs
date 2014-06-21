// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public interface TransitionEventHandler
	{
		void Perform (Animator animator, Dictionary<int, TransitionInfo> transitionInfos);

		/// <summary>
		/// Key string to build hash code so that every handler can be identified within the dictionary. 
		/// Naming convention is:
		/// ClassName[:LayerIndex[:StateHashId]]
		/// </summary>
		/// <returns>Key string as ClassName[:LayerIndex[:StateHashId]].</returns>
		string GetKeyString ();
	}

	public abstract class AbstractTransitionEventHandler : TransitionEventHandler
	{
		public event BaseAnimatorAccess.OnTransitionStartedHandler OnStarted;
		
		public AbstractTransitionEventHandler () {
			if (OnStarted == null) {} // make compiler happy (CS0067)
		}

		public virtual void Perform (Animator animator, Dictionary<int, TransitionInfo> transitionInfos) {
		}

		public override int GetHashCode () {
			return GetKeyString ().GetHashCode ();
		}
		
		public virtual string GetKeyString () {
			return this.GetType ().Name;
		}
	}

	public class AnyTransitionEventHandler : AbstractTransitionEventHandler
	{
		// FIXME_kay: implement Perform - for others too
	}
	
	public class SpecificTransitionEventHandler : AbstractTransitionEventHandler
	{
		public event BaseAnimatorAccess.OnTransitionStartedHandler OnFinished;
		
		protected int layer = -1;
		protected int transitionId = 0;

		public SpecificTransitionEventHandler (int layer, int transitionId)
		{
			if (OnFinished == null) {} // make compiler happy (CS0067)
		}
		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + transitionId;
		}
	}
	
	public class FromStateTransitionEventHandler : AbstractTransitionEventHandler
	{
		protected int layer = -1;
		protected int stateId = 0;

		public FromStateTransitionEventHandler (int layer, int stateId) {
			this.layer = layer;
			this.stateId = stateId;
		}

		public override string GetKeyString () {
			return base.GetKeyString () + ":" + layer + ":" + stateId;
		}
	}
	
}

