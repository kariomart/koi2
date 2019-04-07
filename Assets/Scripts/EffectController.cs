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

    float vignetteDefault = .45f;
    float vignetteMax = .45f;
	float vignetteMin = .40f;

    float bloomDefault = 3.8f;
    float bloomMin = 3.8f;
    float bloomMax = 5.5f;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);
        volume.profile.TryGetSettings(out vignetteLayer);
        volume.profile.TryGetSettings(out colorGradingLayer);

        vignetteLayer.intensity.value = vignetteDefault;
        bloomLayer.intensity.value = bloomDefault;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    
        
    }

    public void setVignette(float depth) {

        float currentVignette = vignetteLayer.intensity;
        float desiredVignette = Mathf.Lerp(vignetteMin, vignetteMax, depth);
        float newVignette = Mathf.MoveTowards(currentVignette, desiredVignette, .0001f);
//        Debug.Log(newVignette);
        vignetteLayer.intensity.value = newVignette;

    }

    public void addBloom(float amt) {
        bloomLayer.intensity.value += amt;
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

}
