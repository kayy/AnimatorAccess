// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;

namespace AnimatorAccessExample
{
	public class Player : MonoBehaviour
	{
		public enum Direction
		{
			Left = -1,
			Staying = 0,
			Right = 1,
		}
		Direction direction = Direction.Staying;
		
		AnimatorAccess.ExamplePlayerAnimatorAccess animA;

		Animator animator;
		int currentState0;
		
		public float YawnThreshold = 0.995f;
		
		public float maxSpeed = 5f;
		public float speed;
		float horizontalInput;
		
		bool jumpKeyPressed = false;
		
		void Awake () {
			animator = GetComponent<Animator> ();
			animA = GetComponent<AnimatorAccess.ExamplePlayerAnimatorAccess> ();
		}
		
		void Update () {
			horizontalInput = Input.GetAxis ("Horizontal");
			jumpKeyPressed = Input.GetKeyDown (KeyCode.UpArrow);
		}
		
		void FixedUpdate () {
			currentState0 = animator.GetCurrentAnimatorStateInfo (0).nameHash;
			if (animA.IsYawn (currentState0)) {
				// input is suppressed on yawning
				speed = 0f;
				return;
			} else if (animA.IsIdle (currentState0)) {
				float random = Random.value;
				if (random > YawnThreshold) {
					animA.SetYawnTrigger ();
				}
			} else if (animA.IsJump (currentState0)) {
				// wait until the jump has finished
				return;
			}
			if (jumpKeyPressed) {
				animA.SetJumpTrigger ();
			}
			float newSpeed = horizontalInput * maxSpeed;
			Direction newDirection = ToDirection (newSpeed);
			if (newDirection != direction) {
				Quaternion rot = Quaternion.Euler (0f, 180f - 90f * (int)newDirection, 0f);
				transform.localRotation = rot;
			}
			speed = newSpeed;
			direction = newDirection;
			rigidbody.MovePosition (transform.position + speed * Vector3.right * Time.deltaTime);
			animA.SetSpeed (Mathf.Abs (speed));
		}
		
		void OnTriggerstay (Collider other) {
			PushBack ();
		}
		
		void OnTriggerEnter (Collider other) {
			PushBack ();
		}
		
		void PushBack () {
			transform.position *= 0.5f;
		}
		
		Direction ToDirection (float s) {
			return s > 0 ? Direction.Right : s < 0f ? Direction.Left : Direction.Staying;
		}
	}
}

