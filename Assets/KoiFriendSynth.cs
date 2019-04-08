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
        int sNote = GameMaster.me.sequencerNote;
        note = minNote + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));

        float strength = Random.Range(.2f, 1.0f);
        float length = Random.Range(.1f, .3f);
        synth.NoteOn(note, strength, length);
        GameMaster.me.sequencer.AddNote(note-12, sNote, sNote+1, strength);
        GameMaster.me.sequencerNote++;

    }

    public void playNote(float length) {

        int sNote = GameMaster.me.whaleSeqNote;
        int note;

        note = minNote + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));

        float strength = Random.Range(.2f, 1.0f);
        synth.NoteOn(note, strength, length);
        GameMaster.me.whaleSequencer.AddNote(note, sNote, sNote+4, Random.Range(.2f,.4f));

    }
}
