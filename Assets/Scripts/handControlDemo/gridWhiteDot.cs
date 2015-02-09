using UnityEngine;
using System.Collections;

public class gridWhiteDot : MonoBehaviour {
	public float speedScale = 1f;

	// Use this for initialization
	void Start () {
		transform.localPosition = new Vector3 (transform.localPosition.x, Random.Range(-9, 9), transform.localPosition.z);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (0, 3f, 0) * speedScale * Time.deltaTime);

		if (transform.localPosition.y > 9) {
			transform.localPosition = new Vector3 (transform.localPosition.x, -9f, transform.localPosition.z);
		}
	}
}
