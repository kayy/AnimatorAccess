// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;

namespace AnimatorAccessExample
{
	/// <summary>
	/// AnimatorAccess based example for player controller.
	/// </summary>
	public class PlayerWithoutEvents : MonoBehaviour
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

		void Update () {
			horizontalInput = Input.GetAxis ("Horizontal");
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				anim.SetJumpTrigger ();
			}
		}

		void FixedUpdate () {
			previousState0 = currentState0;
			currentState0 = animator.GetCurrentAnimatorStateInfo (0).nameHash;
			if (anim.IsYawning (currentState0)) {
				// input is suppressed on yawning
				speed = 0f;
				if (currentState0 != previousState0) {
					// just entered yawning state, play sound
					Debug.Log ("Yawning state entered");
					audio.Play ();
				}
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
			}
			speed = horizontalInput * maxSpeed;
			walkingDirection = ToDirection (speed);
			anim.SetRotate ((int)walkingDirection);
			randomRotationTimestamp = Time.realtimeSinceStartup;
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
