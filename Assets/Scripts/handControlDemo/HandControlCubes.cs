using UnityEngine;
using System.Collections;

public class HandControlCubes : MonoBehaviour {
	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	
	private GameObject[] cube;

	Vector3 leftHand = Vector3.zero;
	Vector3 rightHand = Vector3.zero;
	float[] difference = {0, 0};

	public int resolution = 10;

	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();

		cube = new GameObject[resolution + 1];
		for (int i = 0; i <= resolution; i++) {
			cube[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube[i].transform.Rotate (new Vector3 (Random.Range(0f, 360.0F), Random.Range(0f, 360.0F),Random.Range(0f, 360.0F)));
			cube[i].transform.localScale = new Vector3(Random.Range(1f, 3F), Random.Range(1f, 3F),Random.Range(1f, 3F));
			cube[i].transform.parent = gameObject.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_BodyView.isBodyTracked()) {
			leftHand = _BodyView.SmoothJoint(6); //Left
			rightHand = _BodyView.SmoothJoint(10); //Right

			for (int u = 0; u < 2; u++) {
				difference[u] = rightHand[u] - leftHand[u];
			}
			float[] steps = {(difference[0]/resolution), (difference[1]/resolution)};
			
			for (int i = 0; i <= resolution; i++) {
				cube[i].transform.Rotate (transform.rotation.eulerAngles + new Vector3 ((20F * Time.deltaTime), (20F * Time.deltaTime), 0));
				//keep 1.1f?
				cube[i].transform.localPosition = new Vector3 (i * steps[0], i * steps[1], 0f);
			}
			transform.position = new Vector3(leftHand.x, leftHand.y, 10f);
		}
		else {
			for (int i = 0; i <= resolution; i++) {
				cube[i].transform.localPosition = new Vector3 (0, 0, 0);
				transform.position = new Vector3 (0, 0, 0);
				cube[i].transform.Rotate (transform.rotation.eulerAngles + new Vector3 ((20F * Time.deltaTime), (20F * Time.deltaTime), 0));
			}
		}
	}
}