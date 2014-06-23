// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;


namespace AnimatorAccess {
	/// <summary>
	/// Layer status regarding the current Animator state and transition (if any). Both status values are updated at 
	/// the begining when checking for events is enabled. Previous values are saved automatically when the state or
	/// transition is set. Use layerStatus.State.Current to get the current state and layerStatus.State.Previous to get
	/// the previous value.
	/// </summary>
	public class LayerStatus
	{
		/// <summary>
		/// Stores the current value of T in field previous whenever set is called. This is done even if the new value
		/// equals to current.
		/// </summary>
		public class HistorisedProperty <T> where T : System.IComparable
		{
			T previous = default (T);
			/// <summary>
			/// Gets the previous values.
			/// </summary>
			/// <value>The previous value.</value>
			public T Previous { get { return previous; } }
			
			T current = default (T);
			/// <summary>
			/// Gets or sets a new value to current and saves the last one to previous.
			/// </summary>
			/// <value>The current.</value>
			public T Current {
				get { return current; }
				set {
					previous = current;
					current = value;
				}
			}
			/// <summary>
			/// true if Current != Previous
			/// </summary>
			/// <value><c>true</c> if this instance has changed; otherwise, <c>false</c>.</value>
			public bool HasChanged { get { return !current.Equals (previous); } }
		}
	
		/// <summary>
		/// The layer to consider when checking for changes in states or transitions.
		/// </summary>
		public int Layer;
		/// <summary>
		/// The Animator state, use State.Current to get the current state's hash ID and State.Previous for the 
		/// previous state. Note that 0 is returned on the very first call.
		/// </summary>
		public HistorisedProperty<int> State = new HistorisedProperty<int> ();
		/// <summary>
		/// The transition hash ID or 0 if no transition is active. Use Transition.Current to get the current 
		/// transition's hash ID.
		/// </summary>
		public HistorisedProperty<int> Transition = new HistorisedProperty<int> ();
		
		public LayerStatus (int layer, int state, int transition) {
			this.Layer = layer;
			State.Current = state;
			Transition.Current = transition;
		}
	}
}
