﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleController : MonoBehaviour
{
    Vector2 pos;
    FollowMouse_3D player;
    public float dis;
    public float spd;
    public float range;
    public bool inRange;
    public Vector2 dir;
    KoiFriendSynth synth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameMaster.me.player;
        pos = new Vector2(transform.position.x, transform.position.z);
        synth = GameMaster.me.whaleSynth.gameObject.GetComponent<KoiFriendSynth>();
        dir = player.pos - pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = new Vector2(transform.position.x, transform.position.z);
        dis = Vector2.Distance(player.pos, pos);
        transform.position = new Vector3(pos.x+dir.x*spd, pos.y+dir.y*spd);

        if (dis < range && !inRange) {
            inRange=true;
            synth.playNote(4);
        }
    }
}
