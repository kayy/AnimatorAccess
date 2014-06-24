// The MIT License (MIT)
// 
// Copyright (c) 2014 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using UnityEngine;
using System.Collections.Generic;


namespace AnimatorAccess {
	/// <summary>
	/// Base class for all generated AnimatorAccess classes.
	/// </summary>
	public partial class BaseAnimatorAccess : MonoBehaviour 
	{
		/// <summary>
		/// Callback method for animator state changes.
		/// <param name="info">Details about the affected state. Depending on the context this can be the state just 
		/// entered (OnEnter) or the one just left (OnExit).</param>
		/// <param name="status">Status information about this layer.</param>
		/// </summary>
		public delegate void StateEventHandler (StateInfo info, LayerStatus status);

		/// <summary>
		/// Callback method for transition.
		/// <param name="info">Details about the observed transition.</param>
		/// <param name="status">Status information about this layer.</param>
		/// </summary>
		public delegate void TransitionEventHandler (TransitionInfo info, LayerStatus status);

		/// <summary>
		/// Shortcut for AnyState ().OnChange. Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		public event StateEventHandler OnStateChange {
			add { AnyState ().OnChange += value;}
			remove { AnyState ().OnChange -= value; }
		}

		EventManager _internalEventManager = null;
		protected EventManager EventManager { 
			get {
				if (_internalEventManager == null) {
					_internalEventManager = new EventManager ();
					EventManager.Initialise (animator, this);
				}
				return _internalEventManager;
			}
		}

		public Animator animator;
		
		public Dictionary<int, StateInfo> StateInfos { get { return EventManager.StateInfos; } }

		public Dictionary<int, StateObserver> StateObservers { get { return EventManager.StateObservers; } }

		public Dictionary<int, TransitionInfo> TransitionInfos { get { return EventManager.TransitionInfos; } }

		public Dictionary<int, TransitionObserver> TransitionObservers { get { return EventManager.TransitionObservers; } }

		public LayerStatus [] LayerStatuses { get { return EventManager.LayerStatuses; } }

		public virtual void InitialiseEventManager () {
		}

		/// <summary>
		/// Retrieves a reference to observe a single state.
		/// </summary>
		/// <param name="nameHash">Name hash of the state to observe.</param>
		public SpecificStateObserver State (int nameHash) {
			return EventManager.State (nameHash);
		}

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="layer">If set i.e. != -1, only the speicfied layer will be observed.</param>
		public AnyStateObserver AnyState (int layer = -1) {
			return EventManager.AnyState (layer);
		}
		
		public SpecificTransitionObserver Transition (int source, int dest) {
			return EventManager.Transition (source, dest);
		}

		public FromStateTransitionObserver TransitionFrom (int source) {
			return EventManager.TransitionFrom (source);
		}

		public AnyTransitionObserver AnyTransition () {
			return EventManager.AnyTransition ();
		}

		public string IdToName (int id) { 
			return EventManager.IdToName (id);
		}
		
		/// <summary>
		/// Checks for animator state changes if there are listeners registered in OnStateChange.
		/// </summary>
		/// <param name="animator">Animator instance for reading states of all layers.</param>
		public void CheckForAnimatorStateChanges (Animator animator) {
			EventManager.CheckForAnimatorStateChanges (animator);
		}
	}

}
