﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoiFriend : MonoBehaviour
{

    FollowMouse_3D player;
    ParticleSystem particles;
    AudioSource synthAudio;
    public float dis;
    public float range;
    public Vector2 pos;
    public Vector2 playerPos;
    public Vector3 dir;
    public float speed;
    public float accel;
    public float maxSpeed;

    public KoiFriendSynth sfx;

    public bool chordFish;
    public bool arpFish;
    public bool noteFish;


    // Start is called before the first frame update
    void Start()
    {
        player = GameMaster.me.player;
        sfx = GameMaster.me.friendSynths[Random.Range(0, GameMaster.me.friendSynths.Length)].gameObject.GetComponent<KoiFriendSynth>();
        synthAudio = sfx.gameObject.GetComponent<AudioSource>();
        particles = GetComponent<ParticleSystem>();

        if (Random.Range(0, 100) > 50 ) {
            noteFish = true;
        } else {
            chordFish = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var main = particles.main;
        main.startSize=.3f;

        if (Random.value > .6f) {
            speed+=accel;
        }

        pos = new Vector2(transform.position.x, transform.position.z);
        playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        dis = Vector2.Distance(playerPos, pos);
        speed = Mathf.Clamp(speed, .001f, maxSpeed*dis);

        if (dis > range) {
            Vector2 dirToPlayer = playerPos - pos;
            dir = dirToPlayer.normalized;
            //dir = new Vector2(dirToPlayer.x + Mathf.Sin(Time.time), dirToPlayer.y + Mathf.Sin(Time.time)).normalized;
        }
        dir = new Vector2(dir.x + Mathf.Sin(Time.time)+Random.Range(-.2f,.2f), dir.y + Mathf.Sin(Time.time)+Random.Range(-.2f,.2f)).normalized;
 
        //Vector2 lerpPos = Vector2.Lerp(pos.normalized, dir, speed / (1+(maxSpeed*dis)));
        transform.position = new Vector3(transform.position.x+dir.x*speed, .51f, transform.position.z+dir.y*speed);
       // transform.position = new Vector3(transform.position.x + lerpPos.x*speed, 0.51f, transform.position.z + lerpPos.y*speed);
        speed *= .9f;
        //transform.position = new Vector3(transform.position.x, .51f, transform.position.z);
    }

    public void playSound() {
        Debug.Log(name, synthAudio);
        synthAudio.panStereo = AudioManager.Instance.getPan(transform);

        if (noteFish) {
            sfx.playNote();
        }
        if (chordFish) {
            sfx.playChord();
        }
        
        addSize();
    }

    public void addSize() {
        var main = particles.main;
        main.startSize=1;
    }
}

