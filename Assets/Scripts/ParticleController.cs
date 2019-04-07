using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

	public float lifetime;

	// Use this for initialization
	void Start () {
		
		Destroy(this.gameObject, lifetime);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
