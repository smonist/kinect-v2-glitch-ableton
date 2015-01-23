using UnityEngine;
using System.Collections;

public class GlitchController: MonoBehaviour {
	private Vector3 cubeDirection;
	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	private Vector3 sourceJoint;
	public float grenzwert;
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();
		cubeDirection = Vector3.left;
		sourceJoint = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		sourceJoint = _BodyView.GetLocalAcceleration (true, 10);
		if(sourceJoint.x > grenzwert | sourceJoint.y > grenzwert | sourceJoint.z > grenzwert){
		if (Mathf.Round (Random.Range (1, 100)) == 1) {
			if (Mathf.Round (Random.Range (1, 5)) == 1) {
				transform.localScale = new Vector3 (5, 5, 5);
			} else {
				transform.localScale = new Vector3 (1, 1, 1);
			}
		} else {
			if (Mathf.Round (Random.Range (1, 300)) == 1) {
				transform.localScale = new Vector3 (Random.Range (1, 5),Random.Range (1, 5), Random.Range (1, 5));
			}
		}
		if(Mathf.Round (Random.Range (0, 100)) == 5){
			cubeDirection = new Vector3(Mathf.Round (Random.Range (-1.5f, 1.5f)), Mathf.Round (Random.Range (-1.5f, 1.5f)), Mathf.Round (Random.Range (-1.5f, 1.5f)));
		}

		cubeRotate(cubeDirection);
	}
	}
	
	void cubeRotate (Vector3 direction) {
		transform.Rotate(direction, 40 * Time.deltaTime);
	}
}