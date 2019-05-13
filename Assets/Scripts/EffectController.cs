using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

public class EffectController : MonoBehaviour
{

    PostProcessVolume volume;
    Bloom bloomLayer = null;
    Vignette vignetteLayer = null;
    ColorGrading colorGradingLayer = null;
    ChromaticAberration cA = null;

    float vignetteDefault = .45f;
    float vignetteMax = .45f;
	float vignetteMin = .40f;

    float bloomDefault = 3.8f;
    float bloomMin = 3.8f;
    float bloomMax = 5.5f;

    public float dayHue = 5.99f;
    public float daySat = 14f;
    public float nightSat = -49f;
	public float nightHue = 39f;
    public float dayVig = .504f;
    public float nightVig = .8f;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);
        volume.profile.TryGetSettings(out vignetteLayer);
        volume.profile.TryGetSettings(out colorGradingLayer);
        volume.profile.TryGetSettings(out cA);

        vignetteLayer.intensity.value = vignetteDefault;
        bloomLayer.intensity.value = bloomDefault;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    
        
    }

    public void setHue(float v) {

        if (GameMaster.me.isDay) {
            colorGradingLayer.hueShift.value = Mathf.Lerp(dayHue, nightHue, v);
        } else {
           colorGradingLayer.hueShift.value = Mathf.Lerp(nightHue, dayHue, v); 
        }        

    }

    public void setSat(float v) {

        if (GameMaster.me.isDay) {
            colorGradingLayer.saturation.value = Mathf.Lerp(daySat, nightSat, v);
        } else {
           colorGradingLayer.saturation.value = Mathf.Lerp(nightSat, daySat, v); 
        }        

    }

    public void setVignette(float v) {

        if (GameMaster.me.isDay) {
            vignetteLayer.smoothness.value = Mathf.Lerp(dayVig, nightVig, v);
        } else {
           vignetteLayer.smoothness.value = Mathf.Lerp(nightVig, dayVig, v);
        }      

    }

    public void addVingette(float depth) {

        float currentVignette = vignetteLayer.intensity;
        float desiredVignette = Mathf.Lerp(vignetteMin, vignetteMax, depth);
        float newVignette = Mathf.MoveTowards(currentVignette, desiredVignette, .0001f);
//        Debug.Log(newVignette);
        vignetteLayer.intensity.value = newVignette;

    }

    public void addBloom(float amt) {
        if (bloomLayer.intensity.value < 100) {
            bloomLayer.intensity.value += amt;
        }
//        Debug.Log(bloomLayer.intensity.value>100f);

        if (bloomLayer.intensity >= 2500) {
            Debug.Log("wee");
            GameMaster.me.gameIsOver();
        }
    }

    public void addExposure(float amt) {
        if (colorGradingLayer.postExposure.value < 15) {
            colorGradingLayer.postExposure.value += amt;
        }
    }

    public void setBloom(float depth) {

        // float currentBloom = bloomLayer.intensity;
        // float desiredBloom = Mathf.Lerp(bloomMin, bloomMax, 1-depth);
        // float newBloom = Mathf.MoveTowards(currentBloom, desiredBloom, .01f);
        // bloomLayer.intensity.value = newBloom;
        
        float currentBloom = bloomLayer.intensity.value;
        float desiredBloom = Mathf.MoveTowards(currentBloom, bloomMin, .01f);
        bloomLayer.intensity.value = desiredBloom;
//        Debug.Log(bloomLayer.intensity.value);
    }

    public void StartPumpingBloom() {
        StartCoroutine("PumpBloom");
    }

    public IEnumerator NightTime() {
        float t = 0f;
        float tt=200;
         for (int i = 0; i < tt; i++) {
            t++;
            GameMaster.me.time=0;
            setHue(t/tt);
            setSat(t/tt);
            setVignette(t/tt);
            yield return new WaitForSeconds(.001f);
         }
         GameMaster.me.cycling = false;
         GameMaster.me.isDay = false;
         GameMaster.me.time=0;
    }

    public IEnumerator DayTime() {
        float t = 0f;
        float tt=200;
         for (int i = 0; i < tt; i++) {
            t++;
            GameMaster.me.time=0;
            setHue(t/tt);
            setSat(t/tt);
            setVignette(t/tt);
            yield return new WaitForSeconds(.0001f);
         }
         GameMaster.me.cycling = false;
         GameMaster.me.isDay = true;
         GameMaster.me.time=0;
    }
    public IEnumerator PumpBloom() 
    {   
        for (int i = 0; i < 100; i++) 
        {
            bloomLayer.intensity.value += .05f;
            yield return new WaitForSeconds(.0001f);
        }
        
         for (int i = 0; i < 100; i++) 
        {
            bloomLayer.intensity.value -= .05f;
            yield return new WaitForSeconds(.0001f);
        }
    }

    public IEnumerator PumpCA() 
    {   
        for (int i = 0; i < 50; i++) 
        {
            cA.intensity.value += 0.02f;
            yield return new WaitForSeconds(.0001f);
        }
        
         for (int i = 0; i < 25; i++) 
        {
            cA.intensity.value -= 0.04f;
            yield return new WaitForSeconds(.0001f);
        }
        GameMaster.me.striking = false;
    }

}
