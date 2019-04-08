using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class RainSynth : MonoBehaviour
{

    public bool generating;
    public HelmSequencer sequencer;
    public HelmController synth;
    public int[] scale = { 0, 2, 4, 7, 9 };

    public int minNote = 24;
    public int octaveSpan = 2;
    public float minDensity = 0.5f;
    public float maxDensity = 1.0f;
    public float maxSize = 10.0f;
    int counter;
    int seqNote;

    // Start is called before the first frame

    void Start()
    {
        sequencer = GetComponent<HelmSequencer>();
        synth = GetComponent<HelmController>();
        if (generating) {
            Generate();   
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (generating) {
            if (Mathf.Floor((float)sequencer.GetSequencerPosition()) == 15) {
                Generate();
            }
        }
        //Debug.Log(Mathf.Floor((float)sequencer.GetSequencerPosition())); 
    }

    int GetNote()
    {
        //float size = Random.Range(1f,5f);
        //float octaves = Mathf.Max(0.0f, Mathf.Log(maxSize / size, 2.0f));
        float octaves = 3;
        int playOctave = (int)octaves;
        int scaleNote = (int)(scale.Length * (octaves - playOctave));
        return minNote + playOctave * Utils.kNotesPerOctave + scale[scaleNote];
    }

    int GetKeyFromRandomWalk(int note)
    {
        int octave = note / scale.Length;
        int scalePosition = note % scale.Length;
        return minNote + octave * Utils.kNotesPerOctave + scale[scalePosition];
    }

    int GetNextNote(int current, int max)
    {
        int next = current + Random.Range(-3, 3);

        if (next > max)
            return 2 * max - next;
        if (next < 0)
            return Mathf.Abs(next);

        return next;
    }

    public void Generate()
    {
        sequencer.Clear();

        int maxNote = scale.Length * octaveSpan;
        int currentNote = Random.Range(0, maxNote);
        Note lastNote = sequencer.AddNote(GetKeyFromRandomWalk(currentNote), 0, 1);

        for (int i = 1; i < sequencer.length; ++i)
        {
            float density = Random.Range(minDensity, maxDensity);

            if (Random.Range(0.0f, 1.0f) < density)
            {
                currentNote = GetNextNote(currentNote, maxNote);
                lastNote = sequencer.AddNote(GetKeyFromRandomWalk(currentNote), i, i + 1);
            }
            else
                lastNote.end = i + 1;
        }
    }

    public void playNote() {

        int note;
        // int note = Random.Range(0, 30);
        // int octave = note/scale.Length;
        // int scalePos = note%scale.Length;
        // note = minNote + (octave*12) + scale[scalePos];

        note = minNote + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));

        float strength = Random.Range(.2f, 1.0f);
        float length = Random.Range(.1f, .3f);
        synth.NoteOn(note, strength, length);
        if (seqNote < 256) {
            sequencer.AddNote(note, seqNote,seqNote+1,strength);
        }
        seqNote++;

//        Debug.Log(note);

    }
}
