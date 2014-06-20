// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	public interface TransitionEventHandler
	{
		event BaseAnimatorAccess.OnTransitionStartedHandler Started;

		event BaseAnimatorAccess.OnTransitionStartedHandler Finished;

		/// <summary>
		/// Perform the specified transitionNameHash and transitionInfos.
		/// </summary>
		/// <param name="transitionNameHash">Transition name hash.</param>
		/// <param name="transitionInfos">Transition infos.</param>
		void Perform (Animator animator, Dictionary<int, TransitionInfo> transitionInfos);
	}

	public class BaseTransitionEventHandler : TransitionEventHandler
	{
		public event BaseAnimatorAccess.OnTransitionStartedHandler Started;
		
		public event BaseAnimatorAccess.OnTransitionStartedHandler Finished;
		
		public BaseTransitionEventHandler ()
		{
			Log.Temp ("Constructor");
			if (Started == null || Finished == null) {} // make compiler happy (CS0067)
		}

		public virtual void Perform (Animator animator, Dictionary<int, TransitionInfo> transitionInfos) {

		}
	}

	public class SingleTransitionEventHandler : BaseTransitionEventHandler
	{
		
	}
	
	public class FromStateTransitionEventHandler : BaseTransitionEventHandler
	{
		
	}
	
	public class AnyTransitionEventHandler : BaseTransitionEventHandler
	{
		
	}
	
}

