using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scales : MonoBehaviour {

	public AudioClip[][] pentatonicScale = new AudioClip[5][];
	public AudioClip[][] mixolydianScales = new AudioClip[5][];
	public AudioClip[][] dorianScales = new AudioClip[5][];

	// Use this for initialization
	void Start () {

		pentatonicScale = transform.GetChild(0).gameObject.GetComponent<Scale>().scales;
		//dorianScales = transform.GetChild(1).gameObject.GetComponent<Scale>().scales;
		//mixolydianScales = transform.GetChild(2).gameObject.GetComponent<Scale>().scales;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
