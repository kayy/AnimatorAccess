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
		
		int jumpTrigger;
		int yawnTrigger;
		int speedFloat;
		int idleState;
		int yawnState;
		int jumpState;

		Animator animator;
		int currentState0;
		
		public float YawnThreshold = 0.995f;
		
		public float maxSpeed = 5f;
		public float speed;
		float horizontalInput;
		
		bool jumpKeyPressed = false;
		
		void Awake () {
			animator = GetComponent<Animator> ();

			jumpTrigger = Animator.StringToHash ("JumpTrigger");
			yawnTrigger = Animator.StringToHash ("yawnTrigger");
			speedFloat = Animator.StringToHash ("Speed");
			idleState = Animator.StringToHash ("Base Layer.Idle");
			yawnState = Animator.StringToHash ("Base Layer.Yawn");
			jumpState = Animator.StringToHash ("Base Layer.Jump");
		}
		
		void Update () {
			horizontalInput = Input.GetAxis ("Horizontal");
			jumpKeyPressed = Input.GetKeyDown (KeyCode.UpArrow);
		}
		
		void FixedUpdate () {
			currentState0 = animator.GetCurrentAnimatorStateInfo (0).nameHash;
			if (currentState0 == yawnState) {
				// input is suppressed on yawning
				speed = 0f;
				return;
			} else if (currentState0 == idleState) {
				float random = Random.value;
				if (random > YawnThreshold) {
					animator.SetTrigger (yawnTrigger);
				}
			} else if (currentState0 == jumpState) {
				// wait until the jump has finished
				return;
			}
			if (jumpKeyPressed) {
				animator.SetTrigger (jumpTrigger);
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
			animator.SetFloat (speedFloat, Mathf.Abs (speed));
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

