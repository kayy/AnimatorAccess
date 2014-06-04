using UnityEngine;
using System.Collections;

namespace AnimatorAccessExample
{
	/// <summary>
	/// 'Classic' controller for example player i.e. hash IDs are defined as members and initialised in Awake ().
	/// </summary>
	public class PlayerOld : MonoBehaviour
	{
		public enum Direction
		{
			Left = -1,
			Facing = 0,
			Right = 1,
		}
		Direction walkingDirection = Direction.Facing;

		int jumpTrigger;
		int yawnTrigger;
		int speedFloat;
		int rotateInt;
		int idleState;
		int yawnState;
		int jumpState;

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
			jumpTrigger = Animator.StringToHash ("JumpTrigger");
			yawnTrigger = Animator.StringToHash ("YawnTrigger");
			speedFloat = Animator.StringToHash ("Speed");
			rotateInt = Animator.StringToHash ("Rotate");
			idleState = Animator.StringToHash ("Base Layer.Idle");
			yawnState = Animator.StringToHash ("Base Layer.Yawn");
			jumpState = Animator.StringToHash ("Base Layer.Jump");
			animator.SetInteger (rotateInt, (int)Direction.Facing);
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
					// start yawning from time to time which will block input
					animator.SetTrigger (yawnTrigger);
				}
				if (Time.realtimeSinceStartup - randomRotationTimestamp > randomRotationInterval) {
					// rotate after randomRotationInterval to random direction
					int currrentRotation = animator.GetInteger (rotateInt);
					int newRotation = currrentRotation == 0 ? (int)Mathf.Sign ((int)Random.Range (-1, 1)) : 0;
					animator.SetInteger (rotateInt, newRotation);
					randomRotationTimestamp = Time.realtimeSinceStartup;
				}
			} else if (currentState0 == jumpState) {
				// wait until the jump has finished
				return;
			}
			if (jumpKeyPressed) {
				animator.SetTrigger (jumpTrigger);
			}
			speed = horizontalInput * maxSpeed;
			walkingDirection = ToDirection (speed);
			if (walkingDirection != Direction.Facing) {
				// only turn the player after walking
				animator.SetInteger (rotateInt, (int)walkingDirection);
			}
			rigidbody.MovePosition (transform.position + speed * Vector3.right * Time.deltaTime);
			// if speed != 0, walking animation is triggered
			animator.SetFloat (speedFloat, Mathf.Abs (speed));
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
