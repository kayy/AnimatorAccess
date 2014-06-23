// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;

namespace AnimatorAccessExample
{
	/// <summary>
	/// AnimatorAccess based example for player controller.
	/// </summary>
	public class Player : MonoBehaviour
	{
		public enum Direction
		{
			Left = -1,
			Facing = 0,
			Right = 1,
		}
		Direction walkingDirection = Direction.Facing;
		
		AnimatorAccess.ExamplePlayerAnimatorAccess anim;

		Animator animator;
		int currentState0;

		/// <summary>
		/// Probability for start yawning in idle state. Yawning blocks input.
		/// </summary>
		public float YawnThreshold = 0.995f;

		/// <summary>
		/// In idle state change look direction after interval has elapsed.
		/// </summary>
		public float randomRotationInterval = 3f;
		float randomRotationTimestamp = 0f;

		public float maxSpeed = 5f;
		public float speed;

		/// <summary>
		/// The horizontal input from cursor left and right which is taken in Update (). Transformed to speed in 
		/// FixedUpdate.
		/// </summary>
		float horizontalInput;
		
		bool jumpKeyPressed = false;
		
		void Awake () {
			animator = GetComponent<Animator> ();
			anim = GetComponent<AnimatorAccess.ExamplePlayerAnimatorAccess> ();
			anim.SetRotate ((int)Direction.Facing);
		}

		void OnEnable () {
			anim.AnyState ().OnChange += OnAnyStateChange;
			anim.State (anim.stateIdYawning).OnEnter += OnEnterYawning;
			anim.State (anim.stateIdYawning).OnExit += OnExitYawning;
			anim.AnyTransition ().OnStarted += OnAnyTransitionStarted;
			anim.TransitionFrom (anim.stateIdIdle).OnStarted += OnIdleToAnyState;
			anim.Transition (anim.stateIdIdle, anim.stateIdJumping).OnStarted += OnIdleToJumpingStarted;
			anim.Transition (anim.stateIdIdle, anim.stateIdJumping).OnStay += OnIdleToJumpingStay;
			anim.Transition (anim.stateIdIdle, anim.stateIdJumping).OnFinished += OnIdleToJumpingFinished;
		}
		void OnDisable () {
			anim.AnyState ().OnChange -= OnAnyStateChange;
			anim.State (anim.stateIdYawning).OnEnter -= OnEnterYawning;
			anim.State (anim.stateIdYawning).OnExit -= OnExitYawning;
			anim.AnyTransition ().OnStarted -= OnAnyTransitionStarted;
			anim.TransitionFrom (anim.stateIdIdle).OnStarted -= OnIdleToAnyState;
			anim.Transition (anim.stateIdIdle, anim.stateIdJumping).OnStarted -= OnIdleToJumpingStarted;
			anim.Transition (anim.stateIdIdle, anim.stateIdJumping).OnStay -= OnIdleToJumpingStay;
			anim.Transition (anim.stateIdIdle, anim.stateIdJumping).OnFinished -= OnIdleToJumpingFinished;
		}

		void Update () {
			horizontalInput = Input.GetAxis ("Horizontal");
			jumpKeyPressed = Input.GetKeyDown (KeyCode.UpArrow);
		}

		// state callbacks
		void LogStateChange (string method, AnimatorAccess.StateInfo info, int previous) {
			UnityEngine.Debug.Log (string.Format ("[t={0:0.00}] == '{1:-25}' callback: {2}, previous state was {3}", Time.realtimeSinceStartup, info.Name, method, anim.IdToName (previous)));
		}
		void OnAnyStateChange (AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
			LogStateChange ("OnAnyStateChange", info, status.State.Previous);
		}
		
		void OnEnterYawning (AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
			LogStateChange ("OnEnterYawning", info, status.State.Previous);
		}
		
		void OnExitYawning (AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
			LogStateChange ("OnExitYawning", info, status.State.Previous);
		}

		// transition callbacks
		void LogTransition (string method, AnimatorAccess.TransitionInfo info) {
			UnityEngine.Debug.Log (string.Format ("[t={0:0.00}]     ----> {1:-25} callback: {2}", Time.realtimeSinceStartup, info.Name, method));
		}
		int noOfCallsToOnIdleToJumpingStay = 0;
		void OnIdleToJumpingStarted (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
			noOfCallsToOnIdleToJumpingStay = 0;
			LogTransition ("OnIdleToJumpingStarted", info);
		}
		void OnIdleToJumpingStay (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
			noOfCallsToOnIdleToJumpingStay++;
		}
		void OnIdleToJumpingFinished (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
			Debug.Log ("                     " + noOfCallsToOnIdleToJumpingStay + " calls to OnIdleToJumpingStay");
			LogTransition ("OnIdleToJumpingFinished", info);
		}

		void OnIdleToAnyState (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
			LogTransition ("OnIdleToAnyState", info);
		}
		void OnAnyTransitionStarted (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
			LogTransition ("OnAnyTransitionStarted", info);
		}

		void FixedUpdate () {
			currentState0 = animator.GetCurrentAnimatorStateInfo (0).nameHash;
			if (anim.IsYawning (currentState0)) {
				// input is suppressed on yawning
				speed = 0f;
				return;
			} else if (anim.IsIdle (currentState0)) {
				float random = Random.value;
				if (random > YawnThreshold) {
					// start yawning from time to time which will block input
					anim.SetYawnTrigger ();
				}
				if (Time.realtimeSinceStartup - randomRotationTimestamp > randomRotationInterval) {
					// rotate after randomRotationInterval to random direction
					int currrentRotation = anim.GetRotate ();
					int newRotation = currrentRotation == 0 ? (int)Mathf.Sign ((int)Random.Range (-1, 1)) : 0;
					anim.SetRotate (newRotation);
					randomRotationTimestamp = Time.realtimeSinceStartup;
				}
			} else if (anim.IsJumping (currentState0)) {
				// wait until the jump has finished
				return;
			}
			if (jumpKeyPressed) {
				anim.SetJumpTrigger ();
			}
			speed = horizontalInput * maxSpeed;
			Direction newWalkingDirection = ToDirection (speed);
			if (walkingDirection != Direction.Facing) {
				// only turn the player after walking
				anim.SetRotate ((int)newWalkingDirection);
				randomRotationTimestamp = Time.realtimeSinceStartup;
			}
			walkingDirection = newWalkingDirection;
			rigidbody.MovePosition (transform.position + speed * Vector3.right * Time.deltaTime);
			// if speed != 0, walking animation is triggered
			anim.SetSpeed (Mathf.Abs (speed));
			// alternative (not recommended) way of accessing hash IDs directly:
			// animator.SetFloat (anim.paramIdSpeed, Mathf.Abs (speed));
		}
		
		void OnTriggerstay (Collider other) {
			PushBack ();
		}
		
		void OnTriggerEnter (Collider other) {
			PushBack ();
		}

		/// <summary>
		/// Avoid that player is walking outside camera focus
		/// </summary>
		void PushBack () {
			transform.position *= 0.5f;
		}
		
		Direction ToDirection (float newSpeed) {
			return newSpeed > 0 ? Direction.Right : newSpeed < 0f ? Direction.Left : Direction.Facing;
		}
	}
}

