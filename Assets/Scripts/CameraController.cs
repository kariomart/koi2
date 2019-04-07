using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform targetObject;
	public float followSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {

		transform.position = Vector3.Lerp(transform.position, targetObject.position, Time.deltaTime * followSpeed);
		transform.position = new Vector3(transform.position.x, 75, transform.position.z);

	}
}
