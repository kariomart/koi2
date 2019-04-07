using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public string stateName;
	public AudioClip[] sfx;
	public Scale[] scales;
	public AudioClip baseAmbience;

	// Use this for initialization
	void Start () {

		sfx = Resources.LoadAll<AudioClip>("states/" + stateName + "/sfx/");
		baseAmbience = Resources.Load<AudioClip>("states/" + stateName + "/sfx/ambience");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
