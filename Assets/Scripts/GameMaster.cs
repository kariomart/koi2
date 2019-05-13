﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class GameMaster : MonoBehaviour {

	public static GameMaster me;
	public FollowMouse_3D player;
	public GameState[] gameStates;
	public GameState gameState;
	public int sfxChance;
	public EffectController effects;

	public int rainChance;
	public int lightningChance;
	public int rainStopChance;
	public bool raining;
	public bool lightningStorm;
	public bool striking;

	public int whaleSpawnChance;

	public GameObject rainRipple;
	public RainSynth rainSynthController;
	int dropChance = 150;

	public HelmController rainSynth;
	public AudioSource rainSource;
	public AudioSource lightningSource;
	public HelmController lightningSynth;
	public HelmController koiFriendSynth;
	public HelmController[] friendSynths;
	public HelmController whaleSynth;

	public GameObject koiFriendPrefab;
	public GameObject whalePrefab;

	public HelmSequencer sequencer;
	public HelmSequencer sequencerChords;
	public HelmSequencer whaleSequencer;
	public HelmSequencer rainSequencer;
	public int sequencerNote;
	public int sequencerChordsNote;
	public int whaleSeqNote;
	
	public bool gameover;

	public float time;
	public float dayLength = 200f;
	public bool isDay = true;
	public bool cycling;

	public int waterSize;
	public Transform waterObj;
	public Material waterMat;

	int[] scale = { 0, 2, 4, 7, 9 };

	// Use this for initialization
	void Start () {

		me = this;
		waterMat = waterObj.gameObject.GetComponent<Renderer>().material;
		waterObj.localScale = new Vector3(waterSize,waterSize,waterSize);
		waterMat.mainTextureScale = new Vector2(waterSize, waterSize);
		GameObject g = GameObject.Find("GameStates");
		gameStates = new GameState[g.transform.childCount];
		loadSynths();
		rainSource = rainSynth.gameObject.GetComponent<AudioSource>();
		lightningSource = lightningSynth.gameObject.GetComponent<AudioSource>();
		//spawnFriend();
		for (int i = 0; i < gameStates.Length; i ++) {
			gameStates[i] = g.transform.GetChild(i).GetComponent<GameState>();
		} 


		foreach (Transform c in GameObject.Find("Friends").transform)
		{
			if (c.gameObject.activeInHierarchy) {
				player.friends.Add(c.GetComponent<KoiFriend>());
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!gameover) {
			AudioManager.Instance.updateFilters();
			AudioManager.Instance.updateFX();
			AudioManager.Instance.spatializeSFX();
		}

		if (Input.GetKeyDown(KeyCode.Q)) {
			player.enabled=!player.enabled;
		}

		if (Input.GetKeyDown(KeyCode.W)) {
			sequencer.enabled=!sequencer.enabled;
			sequencerChords.enabled=!sequencerChords.enabled;
			whaleSequencer.enabled=!whaleSequencer.enabled;
			rainSequencer.enabled=!rainSequencer.enabled;
			gameover=true;
		}

		int rand = Random.Range(0, sfxChance);
		
		if (rand == 10) {
			rand = Random.Range(0, 101);
			if (rand > 50) {
				AudioManager.Instance.PlayRandomAboveWaterSFX();
			} else {
				AudioManager.Instance.PlayRandomUnderwaterSFX();
			}
		}

		rand = Random.Range(0, rainChance);
		if (rand == 5 && !raining) {
			AudioManager.Instance.playRain();
			//AudioManager.Instance.scaleNum = 2;
			raining = true;
			rand = Random.Range(0, 4);
			if (rand == 1) {
				lightningStorm=true;
			}
		}

		rand = Random.Range(0, whaleSpawnChance);

		if (rand==1) {
			spawnWhale();
		}

		if (gameover) {
			effects.addBloom(.005f);
			effects.addExposure(.005f);
			AudioManager.Instance.openSequencerFilter();
		}

		DayNightCycle();


		if (raining) {

			rand = Random.Range(0, dropChance);
			int lRand = Random.Range(0, lightningChance);
			if (rand == 0) {
				//Instantiate(rainRipple, player.transform.position, Quaternion.identity);
				GameObject r = Instantiate(rainRipple, new Vector3(player.transform.position.x-7+Random.Range(0f,14f),player.transform.position.y-4+Random.Range(0f,8), player.transform.position.z), Quaternion.identity);
				r.transform.SetParent(rainSynthController.transform);
				rainSource.panStereo = AudioManager.Instance.getPan(r.transform);
				rainSynthController.playNote();
				if (dropChance > 6) {
					dropChance -= 5;
				}
			}

			if (lRand == 1 && !striking && lightningStorm) {
				int note = 12 + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, 1));
				lightningSource.panStereo = Random.Range(-0.8f,0.8f);
				lightningSynth.NoteOn(note, Random.Range(.2f,1), Random.Range(1,2));
				effects.StartCoroutine(effects.PumpCA());
				striking = true;
			}


			rand = Random.Range(0, rainStopChance);
			if (rand == 1 && dropChance<10) {
				raining = false;
				lightningStorm = false;
				AudioManager.Instance.StartCoroutine("FadeOutRain");
				AudioManager.Instance.scaleNum = 0;
			}
		
		}	
		
	}

	public void DayNightCycle() {

		time++;


		if (time > dayLength && isDay && !cycling) {
			effects.StartCoroutine(effects.NightTime());
			Debug.Log("going to night");
			cycling = true;
		}	
		
		if (time > dayLength & !isDay && !cycling) {
			effects.StartCoroutine(effects.DayTime());
			Debug.Log("going to day");
			cycling = true;
		}	


		//effects.setHue(time/dayLength);

	}

	public void loadSynths() {

		GameObject[] synths = GameObject.FindGameObjectsWithTag("FriendSynth");
		int c = 0;
		friendSynths = new HelmController[synths.Length];

		foreach (GameObject s in synths)
		{
			friendSynths[c] = s.GetComponent<HelmController>();
			c++;
		}


	}
	public void enableSequencers() {

		sequencer.enabled=!sequencer.enabled;
		sequencerChords.enabled=!sequencerChords.enabled;
		whaleSequencer.enabled=!whaleSequencer.enabled;
		rainSequencer.enabled=!rainSequencer.enabled;

	}

	public IEnumerator playFriendNotes() {

		float t = .5f * Random.Range(0,2);
		yield return new WaitForSeconds(t);
		List<KoiFriend> tempKoi = new List<KoiFriend>();
		tempKoi = player.friends;
		
		foreach (KoiFriend k in tempKoi)
		{
			//Debug.Log("played note");
			k.playSound();
			player.effects.addBloom(.15f);
			yield return new WaitForSeconds(t);
		}
		//GameMaster.me.sequencerNote++;
	}

	public IEnumerator playFriendChord() {

		float t = 1f;
		yield return new WaitForSeconds(t);
		List<KoiFriend> tempKoi = new List<KoiFriend>();
		tempKoi = player.friends;
		
		foreach (KoiFriend k in tempKoi)
		{
			k.playChordNote();
			player.effects.addBloom(.15f);
			k.StartCoroutine(k.chordSize());
		}
		//GameMaster.me.sequencerNote++;
	}

	public void tryToSpawnFriend() {

		int rand = Random.Range(0,3);

		if (rand == 1) {
			spawnFriend();
			player.friendCounter -= 4;
		}
	}

	public void spawnFriend() {
		GameObject k = Instantiate(koiFriendPrefab, new Vector3(player.pos.x+Random.Range(-20,20), 0.5f, player.pos.y+Random.Range(-20,20)), Quaternion.identity);
		KoiFriend kc = k.GetComponent<KoiFriend>();
		kc.range+=Random.Range(-1,1);
		kc.maxSpeed+=Random.Range(-.05f,.1f);
		player.friends.Add(kc);
		Debug.Log("friend spawned !");
		player.foodCounter=0;
	}

	public void spawnWhale() {

		Instantiate(whalePrefab, new Vector3(player.pos.x+Random.Range(-40,40), 0.5f, player.pos.y+Random.Range(-40,40)), Quaternion.identity);
		//Instantiate(whalePrefab, new Vector3(20,0.5f,-20), Quaternion.identity);

	}

}
