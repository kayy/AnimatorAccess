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
	/// Base class for all generated AnimatorAccess classes. To extend this class use a 2nd partial class definition
	/// outside this file to add methods.
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
		/// Shortcut for AnyState ().OnChange. Occurs once for every change of an animator state in any layer. For 
		/// observing a single layer use AnyState (layer).OnChange instead. To check a specific state use 
		/// State (nameHash).OnXXX
		/// </summary>
		public event StateEventHandler OnStateChange {
			add { AnyState ().OnChange += value;}
			remove { AnyState ().OnChange -= value; }
		}

		/// <summary>
		/// Shortcut for AnyTransition ().OnStarted. Occurs whenever a new transition started in any layer. For 
		/// observing a single layer use AnyTransition (layer).OnStarted instead. See  TransitionFrom (source) and 
		/// Transition (source, destination) for more fine grained events.
		/// </summary>
		public event TransitionEventHandler OnTransitionStarted {
			add { AnyTransition ().OnStarted += value;}
			remove { AnyTransition ().OnStarted -= value; }
		}

		/// <summary>
		/// DON'T USE this directly! Use property EventManager instead.
		/// </summary>
		EventManager _internalEventManager = null;
		/// <summary>
		/// Needed when extending the interface by a second partial class declaration.
		/// </summary>
		/// <value>The event manager.</value>
		protected EventManager EventManager { 
			get {
				if (_internalEventManager == null) {
					_internalEventManager = new EventManager ();
					EventManager.Initialise (animator, this);
				}
				return _internalEventManager;
			}
		}

		/// <summary>
		/// Animator component is set up automatically in the generated class's Awake method.
		/// </summary>
		public Animator animator;

		/// <summary>
		/// State info dictionary having the states' name hashes as key.
		/// </summary>
		public Dictionary<int, StateInfo> StateInfos { get { return EventManager.StateInfos; } }

		/// <summary>
		/// Transition info dictionary having the transitions' name hashes as key.
		/// </summary>
		public Dictionary<int, TransitionInfo> TransitionInfos { get { return EventManager.TransitionInfos; } }

		/// <summary>
		/// Current statuses of all layers, updated on every call to CheckForAnimatorStateChanges ().
		/// </summary>
		/// <value>The layer statuses.</value>
		public LayerStatus [] LayerStatuses { get { return EventManager.LayerStatuses; } }

		public virtual int AllTransitionsHash { get { return 0;} }

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		/// <returns>Handler reference to subscribe to state events.</returns>
		/// <param name="layer">The layer to observe or -1 for all layers.</param>
		public AnyStateHandler AnyState (int layer = -1) {
			return EventManager.AnyState (layer);
		}
		
		/// <summary>
		/// Retrieves a reference to observe one single state specified by its name hash.
		/// </summary>
		/// <returns>Handler reference to subscribe to state events.</returns>
		/// <param name="nameHash">Name hash of the state to observe.</param>
		public SpecificStateHandler State (int nameHash) {
			return EventManager.State (nameHash);
		}
		
		/// <summary>
		/// Observes any transitions of the specified layer or of all layers if layer == -1.
		/// </summary>
		/// <returns>Handler reference to subscribe to transition events.</returns>
		public AnyTransitionHandler AnyTransition (int layer = -1) {
			return EventManager.AnyTransition (layer);
		}
		
		/// <summary>
		/// Observe all transitions from the specified i.e. starting from the given state.
		/// </summary>
		/// <returns>Handler reference to subscribe to transition events.</returns>
		/// <param name="source">Starting point of transition.</param>
		public FromStateTransitionHandler TransitionFrom (int source) {
			return EventManager.TransitionFrom (source);
		}
		
		/// <summary>
		/// Observe all transitions to the specified i.e. ending at the given state.
		/// </summary>
		/// <returns>Handler reference to subscribe to transition events.</returns>
		/// <param name="source">Destination of transition.</param>
		public ToStateTransitionHandler TransitionTo (int destination) {
			return EventManager.TransitionTo (destination);
		}
		
		/// <summary>
		/// Observe only the transition from source to destination.
		/// </summary>
		/// <returns>Handler reference to subscribe to transition events.</returns>
		/// <param name="source">Source.</param>
		/// <param name="destination">Destination.</param>
		public SpecificTransitionHandler Transition (int source, int destination) {
			return EventManager.Transition (source, destination);
		}

		/// <summary>
		/// Generated code of derived classes fill StateInfo and TransitionInfo dictionaries in their overridden method.
		/// </summary>
		public virtual void InitialiseEventManager () {
		}

		/// <summary>
		/// Hash to name conversion.
		/// </summary>
		/// <returns>The state name.</returns>
		/// <param name="stateNameHash">State name hash.</param>
		public string GetStateName (int stateNameHash) { 
			return EventManager.GetStateName (stateNameHash);
		}

		/// <summary>
		/// Hash to name conversion.
		/// </summary>
		/// <returns>The transition name.</returns>
		/// <param name="transitionNameHash">Transition name hash.</param>
		public string GetTransitionName (int transitionNameHash) { 
			return EventManager.GetTransitionName (transitionNameHash);
		}

		/// <summary>
		/// Updates LayerStatuses, checks for status and transition changes and informs all event subscribers.
		/// </summary>
		public void CheckForAnimatorStateChanges () {
			EventManager.CheckForAnimatorStateChanges (animator);
		}
	}

}
