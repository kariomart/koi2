using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider coll) {

		if (coll.gameObject.layer == LayerMask.NameToLayer("Food")) {
			Destroy(coll.gameObject);
			Debug.Log("killed food");
		}

	}
}
