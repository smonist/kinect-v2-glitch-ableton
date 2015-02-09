using UnityEngine;
using System.Collections;

public class gridHorizontalLines : MonoBehaviour {
	public float speedScale = 1f;
	
	void Update () {
		transform.Translate (new Vector3 (0, -3f, 0) * speedScale * Time.deltaTime);

		if (transform.localPosition.y < -9) {
			transform.localPosition = new Vector3 (transform.localPosition.x, 9f, transform.localPosition.z);
		}
	}
}