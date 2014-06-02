using UnityEngine;
using System.Collections;

public class AAPlayerOldStyle : MonoBehaviour 
{
	static int count = 0;
	Animator animator;

	public float acceleration;
	public float deltaAcceleration;
	public float maxAcceleration = 1f;
	public float maxVX = 5f;
	public Vector3 currentVelocity;

	public float speed;

	bool stop;

	void Awake () {
		animator = GetComponent<Animator> ();

	}

	void Update () {
		count++;
		float horizontal = Input.GetAxis("Horizontal");
		speed = horizontal * maxVX;
//		Log.Temp ("horizontal = " + horizontal);
//		if (Input.GetKey (KeyCode.RightArrow)) {
//			acceleration += deltaAcceleration;
//		} else if (Input.GetKey (KeyCode.LeftArrow)) {
//			acceleration -= deltaAcceleration;
//		} else {
//			acceleration = 0f;
//		}
//		stop = acceleration == 0f;
//		acceleration = Mathf.Clamp (acceleration, -maxAcceleration, maxAcceleration);
	}

	void FixedUpdate () {
		rigidbody.MovePosition (transform.position + speed * Vector3.right * Time.deltaTime);

	}
	void OLD_FixedUpdate () {
		currentVelocity = rigidbody.velocity;
		speed = currentVelocity.x;
		if (speed != 0f && stop) {
			Log.Temp (count + ": stop");
			speed *= 0.5f;
		} else {
			speed += acceleration * Time.deltaTime;
		}
//		Log.Temp ("vX = " + vX + " a = " + acceleration);
		speed = Mathf.Clamp (speed, -maxVX, maxVX);
		currentVelocity.x = speed;
		rigidbody.velocity = currentVelocity;
//		animator.SetFloat ("Speed", Mathf.Abs (speed));
	}
}
