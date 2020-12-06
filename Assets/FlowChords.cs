using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class FlowChords : MonoBehaviour
{
    public HelmController synth;
    AudioSource source;
    Camera cam;
    //public AudioMixerGroup mixer;
    public int[] scale = { 0, 2, 4, 7, 9 };
    public int minNote;
    public int octaveSpan ;

    // Start is called before the first frame update
    void Start()
    {
        synth = GetComponent<HelmController>();
        source = GetComponent<AudioSource>();
        cam = Camera.main;
    }

    public void playChord(int offset=0) {

        if (GameMaster.me.player.friends.Count > 0) {
            int rootNote;
            int min = minNote-offset;

            rootNote = min + scale[Random.Range(0,scale.Length)] + (12*Random.Range(0, octaveSpan));
            float strength = Random.Range(.8f, 1.0f);
            int note1 = rootNote+4;
            int note2 = rootNote+7;

            synth.NoteOn(rootNote, strength);
            synth.NoteOn(note1, strength);
            synth.NoteOn(note2, strength);
            GameMaster.me.effects.ShiftHue();
            scaleFriends(true);
        } else {
            GameMaster.me.ResetGame();
        }
    }

    public void stopChord() {

        synth.AllNotesOff();
        scaleFriends(false);
        GameMaster.me.player.RemoveFriend();

    }

    public void scaleFriends(bool val) {

        foreach(KoiFriend kf in GameMaster.me.player.friends) {
            kf.chording = val;
        }
    }

    


    // Update is called once per frame
    void Update()
    {

        if (GameMaster.me.flowMode) {
            if (Input.GetMouseButtonDown(0)) {
                //Debug.Log("noting!");
                playChord(); 
                GameMaster.me.player.spawnRipple(GameMaster.me.player.mouseDeciple.transform.position);
                GameMaster.me.maxFlowTime = 0;
                GameMaster.me.flowTime = Random.Range(0, 60);
            } else if (GameMaster.me.flowTime > GameMaster.me.flowTimer && GameMaster.me.ambientMode) {
                stopChord();
                playChord();
                GameMaster.me.flowTime=0;
            }

            if (Input.GetMouseButtonUp(0)) {
                stopChord(); 
            }

            float mousePos = cam.ScreenToWorldPoint(Input.mousePosition).x - cam.transform.position.x; 
            mousePos = AudioManager.RemapFloat(mousePos, -8, 8, -.8f, .8f);
            mousePos = Mathf.Clamp(mousePos, -.5f, .5f);
            source.panStereo = mousePos;
        }

        
    }
}
