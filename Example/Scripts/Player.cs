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
		
		protected Animator animator;
		int currentState0;
		int previousState0;
		
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
		
		void Awake () {
			animator = GetComponent<Animator> ();
			anim = GetComponent<AnimatorAccess.ExamplePlayerAnimatorAccess> ();
		}

		void Start () {
			anim.SetRotate ((int)Direction.Facing);
		}

		void OnEnable () {
			anim.TransitionTo (anim.stateIdWalking).OnStarted += OnStartedTransitionToWalking;
			anim.State (anim.stateIdIdle).OnActive += OnIdle;
			anim.State (anim.stateIdYawning).OnEnter += OnEnterYawning;
			anim.AnyTransition ().OnStarted += OnAnyTransition;
			anim.AnyTransition (0).OnStarted += OnAnyLayer0Transition;
			anim.AnyTransition (0).OnStarted += OnAnyLayer0TransitionSecondAction;
		}

		void OnDisable () {
			anim.State (anim.stateIdYawning).OnEnter -= OnEnterYawning;
			anim.State (anim.stateIdIdle).OnActive -= OnIdle;
			anim.TransitionTo (anim.stateIdWalking).OnStarted -= OnStartedTransitionToWalking;
			anim.AnyTransition ().OnStarted -= OnAnyTransition;
			anim.AnyTransition (0).OnStarted -= OnAnyLayer0Transition;
			anim.AnyTransition (0).OnStarted -= OnAnyLayer0TransitionSecondAction;
		}

		void OnAnyTransition (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
//			LogTransition ("OnAnyTransition - " + status.Layer, info);
		}
		
		void OnAnyLayer0Transition (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
//			LogTransition ("OnAnyLayer0Transition - " + status.Layer, info);
		}
		
		void OnAnyLayer0TransitionSecondAction (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
//			LogTransition ("OnAnyLayer0TransitionSecondAction - " + status.Layer, info);
		}
		
		void OnEnterYawning (AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
			speed = 0f;
			audio.Play ();
		}

		void OnIdle (AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
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
		}
		
		void OnStartedTransitionToWalking (AnimatorAccess.TransitionInfo info, AnimatorAccess.LayerStatus status) {
			walkingDirection = ToDirection (speed);
			anim.SetRotate ((int)walkingDirection);
			randomRotationTimestamp = Time.realtimeSinceStartup;
//			LogTransition ("OnStartedTransitionToWalking", info);
		}
		
		void Update () {
			horizontalInput = Input.GetAxis ("Horizontal");
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				anim.SetJumpTrigger ();
			}
		}
		
		void FixedUpdate () {
			// input is blocked while yawning
			if (!anim.IsYawning ()) {
				speed = horizontalInput * maxSpeed;
				// if speed != 0, walking animation is triggered
				anim.SetSpeed (Mathf.Abs (speed));
				// alternative (not recommended) way of accessing hash IDs directly:
				// animator.SetFloat (anim.paramIdSpeed, Mathf.Abs (speed));
				rigidbody.MovePosition (transform.position + speed * Vector3.right * Time.deltaTime);
			}
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

#region Debug Utils
		void OnAnyStateChangeDebug (AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
			LogStateChange ("OnAnyStateChange", info, status);
		}
		void LogStateChange (string method, AnimatorAccess.StateInfo info, AnimatorAccess.LayerStatus status) {
			UnityEngine.Debug.Log (string.Format ("[t={0:0.00}] == '{1:-25}' callback: {2}, previous state was {3}", Time.realtimeSinceStartup, info.Name, method, anim.GetStateName (status.State.Previous)));
		}
		void LogTransition (string method, AnimatorAccess.TransitionInfo info) {
			UnityEngine.Debug.Log (string.Format ("[t={0:0.00}]     ----> {1:-25} callback: {2}", Time.realtimeSinceStartup, info.Name, method));
		}
#endregion
	}
	
}
