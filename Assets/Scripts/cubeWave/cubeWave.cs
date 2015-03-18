using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cubeWave : MonoBehaviour {

	public GameObject BodySourceView;
	private BodySourceView _BodyView;

	public GUIText GUIRightHand;

	public int x = 0;
	public int z = 0;
	public float sensitivity = 0;
	int count = 0;
	
	List<GameObject[]> cubes = new List<GameObject[]>();
	
	
	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();

		for (int i = 0; i < x; i++) {
			cubes.Add(new GameObject[z]);
		}
		
		for (int i=0; i < x; i++) {
			for(int u=0; u < z; u++){
				cubes[i][u] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cubes[i][u].transform.localPosition = new Vector3(i, 0, u);
				cubes[i][u].transform.parent = gameObject.transform;
				
				Material mat = Resources.Load("trans", typeof(Material)) as Material;
				cubes[i][u].renderer.material = mat;
				
				cubes[i][u].transform.localScale += new Vector3(0f, 0.5f, 0f);
				float randValue = Random.value;
				if (randValue < .45f) 
				{
					cubes[i][u].transform.localPosition += new Vector3(Random.Range(-0.2f,0.2f), 0, Random.Range(-0.2f,0.2f));
				}
			}		
		}
		gameObject.transform.localPosition = new Vector3 (-x/2f,-15f,20f);
	}
	
	void Update () {
		sensitivity = _BodyView.handAcceleration (true) * 2;

		//sensitivity = Mathf.Clamp (sensitivity, 0f, 10f);

		GUIRightHand.text = sensitivity.ToString ();

		if (sensitivity == null) {
			sensitivity = 0f;
		}


		if (count == x) {
			count = 0;}	
		
		
		
		for (int u = 0; u < z; u++) {
			cubes[count][u].transform.localScale = new Vector3(1f,(Random.Range(-1f, 1f)*sensitivity), 1f);
		}
		
		cubes [Random.Range (0, x)][Random.Range (0, z)].transform.localScale = new Vector3 (1f,(Random.Range(-1f, 1f)*sensitivity), 1f);
		
		count += 1;
		
	}
}