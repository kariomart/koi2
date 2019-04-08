using System.Collections;
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
	public int rainStopChance;
	public int whaleSpawnChance;
	public bool raining;

	public GameObject rainRipple;
	public RainSynth rainSynthController;
	int dropChance = 150;

	public HelmController rainSynth;
	public HelmController koiFriendSynth;
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

	// Use this for initialization
	void Start () {

		me = this;
		GameObject g = GameObject.Find("GameStates");
		gameStates = new GameState[g.transform.childCount];
		//spawnFriend();
		for (int i = 0; i < gameStates.Length; i ++) {
			gameStates[i] = g.transform.GetChild(i).GetComponent<GameState>();
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


		if (raining) {

			rand = Random.Range(0, dropChance);
			if (rand == 0) {
				//Instantiate(rainRipple, player.transform.position, Quaternion.identity);
				GameObject r = Instantiate(rainRipple, new Vector3(player.transform.position.x-7+Random.Range(0f,14f),player.transform.position.y-4+Random.Range(0f,8), player.transform.position.z), Quaternion.identity);
				r.transform.SetParent(rainSynthController.transform);
				rainSynthController.playNote();
				if (dropChance > 6) {
					dropChance -= 5;
				}
			}


			rand = Random.Range(0, rainStopChance);
			if (rand == 1 && dropChance<10) {
				raining = false;
				AudioManager.Instance.StartCoroutine("FadeOutRain");
				AudioManager.Instance.scaleNum = 0;
			}
		
		}	
		
	}
	public void enableSequencers() {

		sequencer.enabled=!sequencer.enabled;
		sequencerChords.enabled=!sequencerChords.enabled;
		whaleSequencer.enabled=!whaleSequencer.enabled;
		rainSequencer.enabled=!rainSequencer.enabled;

	}

	public IEnumerator playFriendNotes() {

		float t = .25f * Random.Range(0,4);
		yield return new WaitForSeconds(t);

		foreach (KoiFriend k in player.friends)
		{
			//Debug.Log("played note");
			k.playNote();
			player.effects.addBloom(.15f);
			yield return new WaitForSeconds(t);
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
