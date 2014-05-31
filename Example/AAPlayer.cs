using UnityEngine;
using System.Collections;

public class AAPlayer : MonoBehaviour 
{
	Animator animator;

	public bool test;
	public bool idle;

	void Awake () {
		animator = GetComponent<Animator> ();

	}
	
	void Update () {
//		Vector3 mousePosition = Input.mousePosition;
		float mouseX = Input.GetAxis ("Mouse X");
		rigidbody.velocity = new Vector3 (mouseX, 0f);
		animator.SetFloat ("Speed", mouseX);
		if (mouseX > 0.1f) {
			Log.Temp ("mouseX : " + mouseX);
			test = true;
			animator.SetBool ("_Test", test);
		} else if (mouseX < -0.1f) {
			Log.Temp ("mouseX : " + mouseX);
			test = false;
			animator.SetBool ("_Test", test);
		} else {
			idle = true;
			animator.SetBool ("Idle", test);
		}
	}
}
