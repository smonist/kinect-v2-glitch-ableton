using UnityEngine;
using System.Collections;

public class HandControl : MonoBehaviour {

	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	
	private ParticleSystem.Particle[] points;
	public int resolution = 10;

	float leftHand;
	float rightHand; 

	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();
		points = new ParticleSystem.Particle[resolution];
	}
	
	// Update is called once per frame
	void Update () {
		if (_BodyView.isBodyTracked()) {
			leftHand = _BodyView.GetJoint(6).x; //Left
			rightHand = _BodyView.GetJoint(10).x; //Right


			float difference = Mathf.Abs(rightHand - leftHand);
			float step = difference/resolution;

			Debug.Log (difference);

			for (int i = 0; i < resolution; i++) {
				points[i].position = new Vector3(i * difference, 0f, 0f);
				points[i].color = new Color(1f, 0f, 0f);
				points[i].size = 1f;
			}

			transform.position = new Vector3(rightHand, 0f, 0f);

			particleSystem.SetParticles(points, points.Length);
		}
		else {
			Debug.Log("NOPE");
		}
	}
}
