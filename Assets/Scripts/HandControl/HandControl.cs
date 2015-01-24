﻿using UnityEngine;
using System.Collections;

public class HandControl : MonoBehaviour {

	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	
	private ParticleSystem.Particle[] points;
	public int resolution = 10;

	float[] leftHand = new float[2];
	float[] rightHand = new float[2];
	float[] difference = {0, 0};

	private Vector3 velocity;
	
	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();

		points = new ParticleSystem.Particle[resolution + 1];
		for (int i = 0; i <= resolution; i++) {
			points [i].color = new Color (1f, 0f, 0f);
		}
		points[2].color = new Color(0f, 0f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		if (_BodyView.isBodyTracked()) {
			Color[] colorCache = new Color[resolution + 1];

			leftHand[0] = _BodyView.GetJoint(6).x; //Left x
			leftHand[1] = _BodyView.GetJoint(6).y; //Left y
			rightHand[0] = _BodyView.GetJoint(10).x; //Right x
			rightHand[1] = _BodyView.GetJoint(10).y; //Right y

			for (int u = 0; u < 2; u++) {
				difference[u] = rightHand[u] - leftHand[u];
			}
			float[] steps = {(difference[0]/resolution), (difference[1]/resolution)};



			for (int i = 0; i <= resolution; i++) {
				points [i].position = new Vector3 (i * steps[0], i * steps[1], 0f);
				points [i].size = 1f;
				colorCache[i] = points[i].color;
			}

			for (int i = 1; i <= resolution; i++) {
				points[i].color = colorCache[i-1];			
			}
			points[0].color = colorCache[resolution];
			

			transform.position = SmoothApproach(transform.position, transform.position, new Vector3(leftHand[0], leftHand[1], 0), 10f);

			particleSystem.SetParticles(points, points.Length);
		}
		else {
			transform.position = new Vector3 (0, 0, 0);
		}
	}

	Vector3 SmoothApproach( Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float speed )
	{
		float t = Time.deltaTime * speed;
		Vector3 v = ( targetPosition - pastTargetPosition ) / t;
		Vector3 f = pastPosition - pastTargetPosition + v;
		return targetPosition - v + f * Mathf.Exp( -t );
	}
}
