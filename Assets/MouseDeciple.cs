using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDeciple : MonoBehaviour
{

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, GameMaster.me.player.transform.position.y, transform.position.z);
    }
}