// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections.Generic;

namespace AnimatorAccess
{
	/// <summary>
	/// Transition handlers are called to check if any transition shoud be reported to listeners. Implementors should 
	/// check for changes in LayerStatus[] and raise an event if their specific condition is met. Note 
	/// that Perform () is called on every FixedUpdate or Update and thus should be carefully implemented regarding 
	/// peformance.  
	/// Transition handlers are stored in a dictionary with GetKeyString () as key to optimise reusing. Implementors 
	/// not deriving from AbstractTransitionHandler have to provide their own GetHashCode () method accordingly.
	/// </summary>
	public interface TransitionHandler
	{
		/// <summary>
		/// Checks if a transition is started, finished, ... and if so raises an event.
		/// </summary>
		/// <param name="statuses">Statuses of all layers.</param>
		/// <param name="transitionInfos">Transition infos to provide to event listeners.</param>
		void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos);

		/// <summary>
		/// Key string to build hash code so that every handler can be identified within the dictionary. 
		/// Naming convention is:
		/// ClassName[:LayerIndex[:StateHashId]]
		/// </summary>
		/// <returns>Key string as ClassName[:LayerIndex[:StateHashId]].</returns>
		string GetKeyString ();
	}

	/// <summary>
	/// Basic implementation of TransitionHandler. Recommended base class for transition handlers.
	/// </summary>
	public abstract class AbstractTransitionHandler : TransitionHandler
	{
		protected AbstractTransitionHandler () {
		}

		public virtual void Perform (LayerStatus[] statuses, Dictionary<int, TransitionInfo> transitionInfos) {
		}

		public override int GetHashCode () {
			return GetKeyString ().GetHashCode ();
		}
		
		public virtual string GetKeyString () {
			return this.GetType ().Name;
		}

		/// <summary>
		/// Gets the TransitionInfo for the speicifed transition ID or null if it is 0 or non-existing.
		/// </summary>
		/// <returns>The transition info.</returns>
		/// <param name="transitionId">Transition identifier to look up.</param>
		/// <param name="transitionInfos">Transition infos to use.</param>
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

	
}

