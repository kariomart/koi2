﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour {

	FollowMouse_3D player;
	bool fading;
	Text t;
	public AudioClip a;
	

	// Use this for initialization
	void Start () {
		player = AudioManager.Instance.player;
		t = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		

		if (Input.GetMouseButtonDown(0)) {
			player.enabled = true;
			AudioManager.Instance.PlaySFX(a, .1f, 1, AudioManager.Instance.abstractAmbience);
			Destroy(this.gameObject, 8f);
			fading = true;
		}

		if (fading) {
			t.color = new Color(t.color.r, t.color.g, t.color.b, t.color.a - .01f);
		}

	}
}
