using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class KoiFriendSynth : MonoBehaviour
{

    public HelmController synth;
    public int[] scale = { 0, 2, 4, 7, 9 };

    public int minNote = 24;
    public int octaveSpan = 2;
    // Start is called before the first frame update
    void Start()
    {
        synth = GetComponent<HelmController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playNote() {

        int note;

        note = minNote + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));

        float strength = Random.Range(.2f, 1.0f);
        float length = Random.Range(.1f, .3f);
        synth.NoteOn(note, strength, length);

    }
}
