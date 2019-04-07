using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

	public float speed;

	Vector2 mouseDir;
	//Vector2 vel;

	SpriteRenderer sprite;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {

		sprite = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		
	}
	
	// Update is called once per frame
	void Update () {

		mouseDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
		//mouseDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
		//Debug.DrawLine(transform.position, Input.mousePosition, Color.magenta);
		
	}

	void FixedUpdate() {

		//rb.MovePosition((Vector2)transform.position + (mouseDir * speed));
	}

	void OnTriggerEnter2D(Collider2D coll) {


		if (coll.gameObject.layer == LayerMask.NameToLayer("Food")) {
			AudioManager.Instance.PlayFoodSound();
			Destroy(coll.gameObject);
		}

	}
}
