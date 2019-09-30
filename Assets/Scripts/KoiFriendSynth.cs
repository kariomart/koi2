using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
using UnityEngine.Audio;

public class KoiFriendSynth : MonoBehaviour
{

    public HelmController synth;
    AudioSource source;
    //public AudioMixerGroup mixer;
    public int[] scale = { 0, 2, 4, 7, 9 };

    public int minNote;
    public int octaveSpan = 2;
    // Start is called before the first frame update
    void Start()
    {
        synth = GetComponent<HelmController>();
        source = GetComponent<AudioSource>();
        minNote = minNote + GameMaster.me.rootNote;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playNote(Vector2 pos1, Vector2 pos2) {

        float dis = Vector2.Distance(pos1, pos2);
        int note;
        int sNote = GameMaster.me.sequencerNote;
        int scNote = GameMaster.me.sequencerChordsNote;

        note = minNote + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));
        //float strength = Random.Range(.2f, 1.0f);
        float strength = 1-(dis/10f);
        //source.volume = 1 - (dis/10f);
        float length = Random.Range(.1f, .3f);
        synth.NoteOn(note, strength, length);

        if (sNote < 128) {
            GameMaster.me.sequencer.AddNote(note-12, sNote, sNote+1, Mathf.Clamp(strength,.2f,.5f));
            GameMaster.me.sequencerNote++;
        } /*else {
            if (!GameMaster.me.gameover) {
                GameMaster.me.gameover=true;
                GameMaster.me.player.sphere.enabled = false;
                //GameMaster.me.gameIsOver();
                GameMaster.me.enableSequencers();
                AudioManager.Instance.sequencers.SetFloat("lowPassFreq", 0);
                AudioManager.Instance.sequencers.SetFloat("Volume",-40);
            }
        }*/

        if (scNote < 32) {
           GameMaster.me.sequencerChords.AddNote(note-31, scNote, scNote+8, Mathf.Clamp(strength,.2f,.5f)); 
        }

        if (sNote % 4 == 0 && sNote != 0) {
            GameMaster.me.sequencerChordsNote+=8;
        }


    }

    public void playNote(float length=.2f) {

        int sNote = GameMaster.me.whaleSeqNote;
        int note;

        note = minNote + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));

        float strength = Random.Range(.2f, 1.0f);
        synth.NoteOn(note, strength, length);

        if (sNote < 16) {
            GameMaster.me.whaleSequencer.AddNote(note, sNote, sNote+4, Random.Range(.2f,.4f));
        }
        GameMaster.me.whaleSeqNote++;
    }

    public void playNote(float length=.2f, int offset=0) {

        int sNote = GameMaster.me.whaleSeqNote;
        int note;

        note = minNote+offset + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));

        float strength = Random.Range(.2f, 1.0f);
        synth.NoteOn(note, strength, length);
        if (sNote < 16) {
            GameMaster.me.whaleSequencer.AddNote(note, sNote, sNote+4, Random.Range(.2f,.4f));
        }
        GameMaster.me.whaleSeqNote++;
    }

    public void playChord(float length=5, int offset=0) {

        int rootNote;
        int min = minNote-offset;
        rootNote = min + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));
        float strength = Random.Range(.8f, 1.0f);
        int note1 = minNote+2;
        int note2 = minNote+4;

        synth.NoteOn(rootNote, strength, length);
        synth.NoteOn(note1, strength, length);
        synth.NoteOn(note2, strength, length);
    }
}
