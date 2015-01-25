using UnityEngine;
using System.Collections;

public class HandControl : MonoBehaviour {

	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	
	private ParticleSystem.Particle[] points;
	public int resolution = 10;

	Vector3 leftHand = Vector3.zero;
	Vector3 rightHand = Vector3.zero;
	float[] difference = {0, 0};

	public float smoothSpeed = 10f;
	Vector3 velocity;
	Vector3 leftHandCache = Vector3.zero;
	Vector3 rightHandCache = Vector3.zero;

	public GameObject leftCube;
	public GameObject rightCube;

	
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
			leftCube.transform.position = _BodyView.GetJoint(6);
			rightCube.transform.position = _BodyView.GetJoint(10);


			Color[] colorCache = new Color[resolution + 1];

			leftHand = _BodyView.GetJoint(6); //Left
			rightHand = _BodyView.GetJoint(10); //Right

			//leftHand = SmoothApproach(points[0].position, leftHandCache, leftHand, smoothSpeed);
			//rightHand = SmoothApproach(points[resolution].position, rightHandCache, rightHand, smoothSpeed);

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
			

			//transform.position = SmoothApproach(transform.position, transform.position, new Vector3(leftHand[0], leftHand[1], 0), 10f);
			transform.position = leftHand;

			particleSystem.SetParticles(points, points.Length);

			
			leftHandCache = _BodyView.GetJoint(6);
			rightHandCache = _BodyView.GetJoint(10);
		}
		else {
			transform.position = new Vector3 (1, 1, 0);
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
