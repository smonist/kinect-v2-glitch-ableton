using UnityEngine;
using System.Collections;

public class PlayerStuff : MonoBehaviour {
	public GameObject BodySourceView;
	private BodySourceView _BodyView;

	private Vector3 acceleration;

	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();
		rigidbody.AddForce(new Vector3(120, 350, 80));
		acceleration = Vector3.zero;
	}
	
	void FixedUpdate () {
		float MoveHorizontal = Input.GetAxis ("Horizontal");
		float MoveVertical = Input.GetAxis ("Vertical");
		
		Vector3 movement = new Vector3(MoveHorizontal, 0.0f, MoveVertical);
		rigidbody.AddForce(movement * 500 * Time.deltaTime);


		acceleration = _BodyView.GetLocalAcceleration(false, 10) * 10;
		if (Mathf.Abs(acceleration.x) > 0.1 | Mathf.Abs(acceleration.y) > 0.1 | Mathf.Abs(acceleration.z) > 0.1) {
			rigidbody.AddForce(new Vector3(acceleration.x, acceleration.y, (acceleration.z * -1)));
		}
	}
}
