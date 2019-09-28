using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
	
	private static AudioManager instance = null;
	public FollowMouse_3D player;
	int lastNotePlayed, lastUnderwaterSoundPlayed, lastAboveWaterSoundPlayed, lastRainPlayed = -1;

	public AudioSource rainSource;
	//we need an SFX Prefab - these will be instantiated for the purpose of playing sounds
	[SerializeField] GameObject myPrefabSFX;
	
	//we also need two audio sources for game music and menu music
	[SerializeField] AudioSource gameMusicAudioSource;
	public int scaleNum;
	public AudioClip[][] scales = new AudioClip[4][]; //, menuMusicAudioSource;

	//this is where our game SFX are going to live

	[Header("SFX")]
	//public AudioClip[] aboveWaterSFX;
	public List<AudioClip> aboveWaterSFX;
	public List<AudioClip> underwaterSFX;
	public AudioClip[] rainSFX;
	
	//clips that play when we activate different objects
	[Header("Food Sounds")]
	public AudioClip[] mixolydianScale;
	public AudioClip[] pentatonicScaleC3;
	public AudioClip[] pentatonicScaleC2;
	public AudioClip[] dorianScale;

	[Header("Ambient Drones")]
	public AudioClip[] friendDroneNotes;

	
	
	//our audio mixer groups, which we are routing our sfx to
	[Header("Mixer Groups")] 
	public AudioMixerGroup abstractAmbience;
	public AudioMixerGroup sfxMixer;
	public AudioMixerGroup underwaterSFXMixer;
	public AudioMixerGroup sequencerMixers;
	public AudioMixer ambienceMaster;
	public AudioMixer sfxMaster;
	public AudioMixer underwaterSFXMaster;
	public AudioMixer sequencers;

	//Mixer snapshots let us crossfade easily between game states.
	//We can also add weights to multiple snapshots in order to blend them.
	// [Header("Mixer Snapshots")] 
	// public AudioMixerSnapshot menuMixerSnapshot;
	// public AudioMixerSnapshot gameMixerSnapshot;
	float lowPassMin = 500f;
	float lowPassMax = 8000f;
	float chorusMax = .5f;
	float distortionMax = 0.1f;

	float sfxVolMin = -30;
	float sfxVolMax = -22;

	public EffectController FX;
	public TextMesh debug;
	public List<AudioSource> activeSFX = new List<AudioSource>();

	

	public static AudioManager Instance {
		get { 
			return instance;
		}
	}

	void Awake () {
		instanceMe();

		//DontDestroyOnLoad(this.gameObject);
		scales[0] = pentatonicScaleC3;
		scales[1] = mixolydianScale;
		scales[2] = dorianScale;
		scales[3] = pentatonicScaleC2;

		ambienceMaster = abstractAmbience.audioMixer;
		sfxMaster = sfxMixer.audioMixer;
		underwaterSFXMaster = underwaterSFXMixer.audioMixer;
		sequencers = sequencerMixers.audioMixer;
		setMixDefaults();
		// if (SceneManager.GetActiveScene().name == "Menu") {
		// 	StartMenu();
		// }

		
	}

	public void instanceMe() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
	}
	//========================================================================

	
	//In tour, once we activate a site, it illuminates and plays a tone.
	//This is currently set up so that we randomly select a sound effect from the corresponding array,
	//Then route it to the appropriate group in the mixer
	public void PlayFoodSound() {
		//Plays a random site sound when we find a site (one of the large circles)
		AudioClip foodSound;
		//First find a clip randomly from the array
		AudioClip[] scale = scales[scaleNum];
//		Debug.Log(scale.Length);

		int rand = Random.Range(0, scale.Length);

		while (rand == lastNotePlayed) {
			rand = Random.Range(0, scale.Length);
		}

		lastNotePlayed = rand;
		foodSound = scale[rand];	
//		Debug.Log(dis);
		
		//Then we play this clip - note that nothing is changing for panning and volume is set at 1.0
		PlaySFX(foodSound, 1.0f, getPan(), 5f, abstractAmbience);
	}

	public void PlayRandomAboveWaterSFX() {
		AudioClip sfx;
		int rand = Random.Range(0, aboveWaterSFX.Count);
		sfx = aboveWaterSFX[rand];	
		aboveWaterSFX.Remove(sfx);
		CreateRandomSound(sfx, 1.0f, 1, sfxMixer);
	}

	public void PlayRandomUnderwaterSFX() {
		AudioClip sfx;
		int rand = Random.Range(0, underwaterSFX.Count);
		sfx = underwaterSFX[rand];	
		underwaterSFX.Remove(sfx);
		CreateRandomSound(sfx, 1.0f, 1, underwaterSFXMixer);
	}

	public void PlayRandomWeatherSFX() {
		
		AudioClip sfx;
		GameState state = GameMaster.me.gameState;
		sfx = state.sfx[Random.Range(0, state.sfx.Length)];
		//PlaySFX(sfx, 1f, getPan(), sfxMixer);
		//PlayWeatherSFX(sfx, 1f, 1f, sfxMixer);

	}

	public void playRain() {

		AudioClip sfx;
		//First find a clip randomly from the array
		int rand = Random.Range(0, rainSFX.Length);

		while (rand == lastRainPlayed) {
			rand = Random.Range(0, rainSFX.Length);
		}

		lastRainPlayed = rand;
		sfx = rainSFX[rand];	
		rainSource.clip = sfx;
		rainSource.Play();
		StartCoroutine("FadeInRain");

	}


	public IEnumerator FadeOutRain() {

     for (float v = rainSource.volume; v > 0f; v -= Time.deltaTime / 2.0f) 
     {
         rainSource.volume = v;
         yield return null;
     }
     rainSource.volume = 0;
 	}

	 public IEnumerator FadeInRain() {

     for (float v = rainSource.volume; v < 1f; v += Time.deltaTime / 2.0f) 
     {
         rainSource.volume = v;
         yield return null;
     }
     rainSource.volume = 1;
 	}
	//========================================================================
	
	
	//This is a general method to instantiate our SFX prefab with the settings that we want, then destroy it when it's
	//done playing
	public void PlaySFX (AudioClip g_SFX, float g_Volume, float g_Pan, AudioMixerGroup g_destGroup) {
		GameObject t_SFX = Instantiate (myPrefabSFX, GameMaster.me.SFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		AudioSource source = t_SFX.GetComponent<AudioSource> ();
		source.clip = g_SFX;
		source.volume = g_Volume;
		//source.panStereo = g_Pan;
		source.outputAudioMixerGroup = g_destGroup;
		source.Play ();
		Destroy(t_SFX.gameObject, g_SFX.length);
	}

	public void PlaySFX (AudioClip g_SFX, float g_Volume, float g_Pan, float g_distance, AudioMixerGroup g_destGroup) {
		GameObject t_SFX = Instantiate (myPrefabSFX, GameMaster.me.SFX) as GameObject;
		t_SFX.name = "SFX_" + g_SFX.name;
		AudioSource source = t_SFX.GetComponent<AudioSource> ();
		source.clip = g_SFX;
		source.volume = g_Volume;
		//source.panStereo = g_Pan;
		source.minDistance = g_distance;
		source.outputAudioMixerGroup = g_destGroup;
		source.Play ();
		Destroy(t_SFX.gameObject, g_SFX.length);
	}

	public void CreateRandomSound (AudioClip g_SFX, float g_Volume, float g_Pan, AudioMixerGroup g_destGroup) {
		GameObject t_SFX = Instantiate (myPrefabSFX, new Vector3(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y,  player.transform.position.z + Random.Range(-10f, 10f)), Quaternion.identity);
		t_SFX.transform.SetParent(GameMaster.me.SFX);
		t_SFX.name = "SFX_" + g_SFX.name;
		AudioSource source = t_SFX.GetComponent<AudioSource> ();
		activeSFX.Add(source);
		source.clip = g_SFX;
		source.volume = g_Volume;
		source.panStereo = getPan(t_SFX.transform);
		source.outputAudioMixerGroup = g_destGroup;
		source.Play ();
		//Destroy(t_SFX.gameObject, g_SFX.length);
	}

	public void spatializeSFX() {

		foreach (AudioSource a in activeSFX)
		{
			a.panStereo = getPan(a.transform);
			//Debug.Log("panning " + a.gameObject.name + " " + getPan(a.transform));
			a.volume = getVol(a.transform);
			
		}
	}

	public void setMixDefaults() {

		sfxMaster.SetFloat("volume", -30f);
		sfxMaster.SetFloat("lowPassFreq", 500f);
		sfxMaster.SetFloat("distortionLevel", 0.1f);
		sfxMaster.SetFloat("chorusMix", .5f);

	}

	public void openSequencerFilter() {

		float lowPassVal, vol, sfxvol, usfxvol;
		sequencers.GetFloat("lowPassFreq", out lowPassVal);

		sequencers.GetFloat("Volume", out vol);
		sfxMaster.GetFloat("volume", out sfxvol);
		underwaterSFXMaster.GetFloat("volume", out usfxvol);

		if (vol < 0) {
			sequencers.SetFloat("Volume", vol+=0.05f);
		}
 

		sequencers.SetFloat("lowPassFreq", lowPassVal+=3f);

		sfxMaster.SetFloat("volume", sfxvol-=0.1f);
		underwaterSFXMaster.SetFloat("Volume", usfxvol-=0.1f);

	}

	
	public void updateFX() {

		FX.addVingette(player.depthPercentage);
		FX.setBloom(player.depthPercentage);

	}

	public void updateFilters() {

		float volVal, lowPassVal, distortionVal, chorusVal;

		sfxMaster.GetFloat("volume", out volVal);
		sfxMaster.GetFloat("lowPassFreq", out lowPassVal);
		sfxMaster.GetFloat("distortionLevel", out distortionVal);
		sfxMaster.GetFloat("chorusMix", out chorusVal);
//		Debug.Log(player.depthPercentage);

		float desiredVol = Mathf.Lerp(sfxVolMin, sfxVolMax, 1 - player.depthPercentage);
		float newVol = Mathf.MoveTowards(volVal, desiredVol, 1f);

		float desiredLowPass = Mathf.Lerp(lowPassMin, lowPassMax, 1 - player.depthPercentage);
		float newLowpassVal = Mathf.MoveTowards(lowPassVal, desiredLowPass, 25f);

		float desiredChorus = Mathf.Lerp(0, chorusMax, player.depthPercentage);
		float newChorusVal = Mathf.MoveTowards(chorusVal, desiredChorus, .05f);

		float desiredDistortion = Mathf.Lerp(0, distortionMax, player.depthPercentage);
		float newDistortionVal = Mathf.MoveTowards(distortionVal, desiredDistortion, .05f);

		sfxMaster.SetFloat("volume", newVol);
		sfxMaster.SetFloat("lowPassFreq", newLowpassVal);
		sfxMaster.SetFloat("distortionLevel", newDistortionVal);
		sfxMaster.SetFloat("chorusMix", newChorusVal);

	}


	public void openFilter() {

		float val;
		sfxMaster.GetFloat("lowPassFreq", out val);

		if(val < lowPassMax) {
			sfxMaster.SetFloat("lowPassFreq", val += 5);
			Debug.Log("opening filter..." + val);
		} else {
			GameMaster.me.player.goingUp = false;
			GameMaster.me.player.goingDown = true;
			PlayFoodSound();
		}
	}

	public void closeFilter() {

		float lpVal;
		 sfxMaster.GetFloat("lowPassFreq", out lpVal);
		if(lpVal > lowPassMin) {
			sfxMaster.SetFloat("lowPassFreq", lpVal -= 5);
		} else {
			GameMaster.me.player.goingDown = false;
		}
	}

	public void LowerSFXOctave() {

		float currentPitch;
		ambienceMaster.GetFloat("pitch", out currentPitch);
		float newPitch = currentPitch - Mathf.Pow(1.05946f, 14);
		ambienceMaster.SetFloat("pitch",  newPitch);

	}

	public void RaiseSFXOctave() {

		float currentPitch;
		ambienceMaster.GetFloat("pitch", out currentPitch);
		float newPitch = currentPitch + Mathf.Pow(1.05946f, 12);
		ambienceMaster.SetFloat("pitch",  newPitch);

	}

	public float getVol(Transform t) {

		float dis = Vector3.Distance(t.position, player.transform.position);
		float vol = RemapFloat(dis, 0, 10, 0, 1);
		return vol;
	}

	public float getPan() {

		float dis = Vector3.Distance(Camera.main.transform.position, player.transform.position);
		if (player.transform.position.x < Camera.main.transform.position.x) {dis *= -1;}
		float pan = Mathf.Clamp(dis /= 5f, -1, 1);
		return pan;
	}

	public float getPan(Transform o) {

		float dis = Vector3.Distance(player.transform.position, o.position);
		if (o.position.x < player.transform.position.x) {dis *= -1;}
		float pan = RemapFloat(dis, -10, 10, -.5f, .5f);

		return pan;
	}

	public void updateDebug() {

		float val;
		sfxMaster.GetFloat("lowPassFreq", out val);

		debug.text = "depth: " + player.depth + "/" + player.desiredDepth;

	}



	// public void StartGame() {
	// 	if (gameMusicAudioSource != null && !gameMusicAudioSource.isPlaying) {
	// 		Debug.Log("Play Game Music");
	// 		gameMusicAudioSource.outputAudioMixerGroup = gameMusicGroup;
	// 		gameMusicAudioSource.Play();
	// 	}

	// 	if (gameMixerSnapshot != null) {
	// 		gameMixerSnapshot.TransitionTo(1.0f);
	// 	}
		
	// }

	
	
	//I've included a remap float function, which might be useful if you want to scale audio values based
	//on arbitrary gameplay ranges
	public static float RemapFloat (float inputValue, float inputLow, float inputHigh, float outputLow, float outputHigh) {
		return (inputValue - inputLow) / (inputHigh - inputLow) * (outputHigh - outputLow) + outputLow;
	}

	public void setScale(int index = 0) {

		scaleNum = index;

	}
	
}
