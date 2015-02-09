using UnityEngine;
using System.Collections;

public class verticalGridController : MonoBehaviour {
	public GameObject BodySourceView;
	private BodySourceView _BodyView;

	private float playerOffset = 0.0f;
	public float angleMultiplier = 0.8f;
	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();
	}
	
	// Update is called once per frame
	void Update () {
		playerOffset = _BodyView.SmoothJoint (0).x;

		transform.eulerAngles = new Vector3 (0, (playerOffset * -angleMultiplier), 0);
	}
}