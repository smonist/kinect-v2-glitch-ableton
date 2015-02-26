using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {
	public Text asd;

	// Use this for initialization
	void Start () {

	}

	string test = "";
	// Update is called once per frame
	void Update () {
		asd.text = "";

		for(int i=0;i<885;i++){
			if(Random.Range(0,3)==0){
				test ="h";
			}
			if(Random.Range(0,3)==1){
				test ="3";
			}
			if(Random.Range(0,3)==2){
				test ="N";
			}

			asd.text += test;
		};


	}
}
