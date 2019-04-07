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

	public int rainChance;
	public int rainStopChance;
	public bool raining;

	public GameObject rainRipple;
	public RainSynth rainSynth;
	int dropChance = 150;



	// Use this for initialization
	void Start () {

		me = this;
		GameObject g = GameObject.Find("GameStates");
		gameStates = new GameState[g.transform.childCount];

		for (int i = 0; i < gameStates.Length; i ++) {
			gameStates[i] = g.transform.GetChild(i).GetComponent<GameState>();
		} 
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		AudioManager.Instance.updateFilters();
        AudioManager.Instance.updateFX();
		AudioManager.Instance.spatializeSFX();

		if (Input.GetKeyDown(KeyCode.Q)) {
			player.enabled=!player.enabled;
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
			AudioManager.Instance.scaleNum = 2;
			raining = true;
		}



		if (raining) {

			rand = Random.Range(0, dropChance);
			if (rand == 0) {
				//Instantiate(rainRipple, player.transform.position, Quaternion.identity);
				GameObject r = Instantiate(rainRipple, new Vector3(player.transform.position.x-7+Random.Range(0f,14f),player.transform.position.y-4+Random.Range(0f,8), player.transform.position.z), Quaternion.identity);
				r.transform.SetParent(rainSynth.transform);
				rainSynth.playNote();
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

	// public void playFriendNotes() {

	// 	foreach (KoiFriend k in player.friends)
	// 	{
	// 		//Debug.Log("played note");
	// 		k.playNote();
	// 	}
	// }

	public IEnumerator playFriendNotes() {

		float t = .25f * Random.Range(0,4);
		yield return new WaitForSeconds(t);
		foreach (KoiFriend k in player.friends)
		{
			//Debug.Log("played note");
			k.playNote();
			player.effects.addBloom(.3f);
			yield return new WaitForSeconds(t);
		}

	}

}
