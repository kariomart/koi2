using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour {

	public string scaleName = "pentatonic";
	public AudioClip[][] scales = new AudioClip[5][];
	public AudioClip[] chords = new AudioClip[10];
	public AudioClip[] scale1;
	public AudioClip[] scale2;
	public AudioClip[] scale3;
	public AudioClip[] scale4;
	public AudioClip[] scale5;


	// Use this for initialization
	void Start () {

		scale1 = Resources.LoadAll<AudioClip>("scales/" + scaleName + "/c1");
		scale2 = Resources.LoadAll<AudioClip>("scales/" + scaleName + "/c2");
		scale3 = Resources.LoadAll<AudioClip>("scales/" + scaleName + "/c3");
		scale4 = Resources.LoadAll<AudioClip>("scales/" + scaleName + "/c4");
		scale5 = Resources.LoadAll<AudioClip>("scales/" + scaleName + "/c5");
		scales[0] = scale1;
		scales[1] = scale2;
		scales[2] = scale3;
		scales[3] = scale4;
		scales[4] = scale5;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
