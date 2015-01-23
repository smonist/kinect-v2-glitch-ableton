using UnityEngine;
using System.Collections;

public class Ulrichs_Block : MonoBehaviour {
	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	
	private Vector3 acceleration;
	
	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();
		acceleration = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		acceleration = _BodyView.GetLocalAcceleration(true, 10);
		acceleration = V3Abs(acceleration);
		
		
		if (acceleration.x > 0.3 | acceleration.y > 0.3 | acceleration.z > 0.3) {
			transform.localScale += new Vector3 (0.02F, 0, 0);
		}
		else {
			if (transform.lossyScale [0] > 0) {
				transform.localScale -= new Vector3 (0.01F, 0, 0);
			}
		}
	}


	private Vector3 V3Abs(Vector3 input) {
		input.Set(Mathf.Abs(input[0]), Mathf.Abs(input[1]), Mathf.Abs(input[2]));
		return input;
	}
}