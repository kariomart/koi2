using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

    public float moveSpeed = 0.004f;
    Camera cam;
    Transform reticle;

    AudioSource waterSfx;
    // Use this for initialization
    void Start () {

        reticle = GameObject.Find("reticle").transform;
        Cursor.visible = false;
        cam = Camera.main;
        waterSfx = GetComponent<AudioSource>();
    }
  
  // Update is called once per frame
  void FixedUpdate () {

        //checkPosition();
        waterSounds();
        Vector2 mouseDir = (cam.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        reticle.position = Vector2.Lerp(transform.position, cam.ScreenToWorldPoint(Input.mousePosition), moveSpeed) + mouseDir * 2;
        transform.position = Vector2.Lerp(transform.position, cam.ScreenToWorldPoint(Input.mousePosition), moveSpeed);

        Vector3 difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z - 90f);

        if (Input.GetKeyDown(KeyCode.Space)) {
            int rand  = Random.Range(0, AudioManager.Instance.scales.Length);

            while (rand == AudioManager.Instance.scaleNum) {
				rand = Random.Range(0, AudioManager.Instance.scales.Length);
			}
            AudioManager.Instance.scaleNum = rand;
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            //AudioManager.Instance.scaleNum = AudioManager.Instance.scaleNum % AudioManager.Instance.scales.Length;

        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            AudioManager.Instance.LowerSFXOctave();
            
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            AudioManager.Instance.RaiseSFXOctave();
            
        }
    }

    void checkPosition() {

        if (Mathf.Abs(transform.position.x) > MapGenerator.me.tileSize * MapGenerator.me.xTiles / 2) {
            transform.position = Vector2.zero;
        }

        if (Mathf.Abs(transform.position.y) > MapGenerator.me.tileSize * MapGenerator.me.yTiles / 2) {
            transform.position = Vector2.zero;
        }
    }

    void waterSounds() {

        float dis = Vector2.Distance(transform.position, cam.transform.position); 
        float vol = AudioManager.RemapFloat(dis, 0, 4.5f, 0, 1f);
        waterSfx.volume = vol;

		if (transform.position.x < Camera.main.transform.position.x) {dis *= -1;}
		float pan = AudioManager.RemapFloat(dis, -5f, 5f, -1f, 1f);
        waterSfx.panStereo = pan;


    }


    void OnTriggerEnter2D(Collider2D coll) {

		if (coll.gameObject.layer == LayerMask.NameToLayer("Food")) {
			AudioManager.Instance.PlayFoodSound();
			Destroy(coll.gameObject);
		}

	}
	
}
