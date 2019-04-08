using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse_3D : MonoBehaviour {

    public float moveSpeed = 0.004f;
    public Vector2 pos;
    Camera cam;
    Transform reticle;
    AudioSource waterSfx;
    public SphereCollider sphere;
    public GameObject foodRipple;
    public bool goingUp;
    public bool goingDown;
    int timeToSurface;
    int goingToSurfaceTimer;

    public float depth;
    public float desiredDepth;
    public float depthPercentage;
    public float desiredScale;
    public float maxDepth;
    public float minDepth;

    public float maxScale;
    public float minScale;

    public float maxDis;

    GameObject closestFood;
    public List<GameObject> foods = new List<GameObject>();
    public float disToFood;
    public LineRenderer line;
    ParticleSystem FX;
    public GameObject water;
    Material waterShader;
    public EffectController effects;

    public List<KoiFriend> friends = new List<KoiFriend>();
    float bloomAmt = .25f;
    public int foodCounter;
    public int friendCounter = 16;


    // Use this for initialization
    void Start () {
        
        reticle = GameObject.Find("reticle").transform;
        sphere = GetComponent<SphereCollider>();
        Cursor.visible = false;
        cam = Camera.main;
        waterSfx = GetComponent<AudioSource>();
        line = GetComponent<LineRenderer>();
        depthPercentage = 1;
        FX = GetComponent<ParticleSystem>();
        waterShader = water.GetComponent<Renderer>().material;
        
    }
  
  // Update is called once per frame
  void FixedUpdate () {

        pos = new Vector2(transform.position.x, transform.position.z);
        //checkPosition();
        waterSounds();
        //spawnRipple();
        if (foods.Count > 2) {
            checkFood();
        }


        AudioManager.Instance.updateDebug();
        Vector3 mouseDir = (cam.ScreenToWorldPoint(Input.mousePosition) - transform.position);
		mouseDir.y = 0.51f;
        //reticle.position = Vector2.Lerp(transform.position, cam.ScreenToWorldPoint(Input.mousePosition), moveSpeed) + mouseDir * 2;
        //transform.position = Vector3.Lerp(transform.position, mouseDir, moveSpeed);
		transform.position = transform.position + mouseDir * moveSpeed;
        transform.position = new Vector3(transform.position.x, .51f, transform.position.z);
        //water.transform.position = transform.position;
        //waterShader.SetVector("_Offset", new Vector4(transform.position.x, transform.position.z, 0, 0));


        //float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, rotation_z - 90f);

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

        // if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
        //     AudioManager.Instance.LowerSFXOctave();
            
        // }

        // if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
        //     AudioManager.Instance.RaiseSFXOctave();
        // }

        // if (goingUp) {
        //     if (depth > minDepth) {
        //         depth --;
        //     } else {
        //         goingUp = false;
        //         goingDown = true;
        //     }
        // }

        // if (goingDown) {
        //     if (depth < maxDepth) {
        //         depth ++;
        //     } else {
        //         goingDown = false;
        //     }
        // }

        float scale = transform.localScale.x;

        if (closestFood) {
            if (line.enabled) {
                debugLine();
            }
            Vector2 foodPos = new Vector2(closestFood.transform.position.x, closestFood.transform.position.z);
            Vector2 koiPos = new Vector2(transform.position.x, transform.position.z); 
            float dis = Vector2.Distance(koiPos, foodPos);
            disToFood = 1 - (dis / maxDis);
            desiredDepth = Mathf.Lerp(minDepth, maxDepth, disToFood);

        } else {

            desiredDepth = 0;
        }

        depth = Mathf.MoveTowards(depth, desiredDepth, 1f);
        //Debug.Log("DESIRED DEPTH: " + depth + "\n"  )
        desiredScale = Mathf.Lerp(minScale, maxScale, depth / maxDepth);
        //Debug.Log("SCALE: " + transform.localScale.x + "\n DESIRED SCALE: " + desiredScale); 
        scale = Mathf.MoveTowards(scale, desiredScale, 1f);
        transform.localScale = new Vector3(scale, scale, scale);
        
        depthPercentage = (1 - (depth + 1f) / 200f);
        if (depthPercentage <= .75f) {
            depthPercentage = 0;
        }

        if (depthPercentage >= .95f) {
            depthPercentage = 1;
        }

        if (depthPercentage == 0) {
            var main = FX.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(.5f, 1.25f);
        } else {
            var main = FX.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 2f);
        }

       //Debug.Log(depthPercentage);
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

        float vol = ((1 - (depth + 1f) / 200f)) / 10f;
        if (vol <= .045f) {
            vol = .015f;
        }
        waterSfx.volume = vol;

		// if (transform.position.x < Camera.main.transform.position.x) {dis *= -1;}
		// float pan = AudioManager.RemapFloat(dis, -5f, 5f, -1f, 1f);
        // waterSfx.panStereo = pan;
    }


    void OnTriggerEnter(Collider coll) {

		if (coll.gameObject.layer == LayerMask.NameToLayer("Food")) {
            AudioManager.Instance.PlayFoodSound();
            foodCounter++;
            if (friends.Count > 0) {
                GameMaster.me.StartCoroutine("playFriendNotes");
            }
            //AudioManager.Instance.FX.StartPumpingBloom();
            effects.addBloom(bloomAmt);
			Destroy(coll.gameObject);

            if (foodCounter >= friendCounter) {
                GameMaster.me.tryToSpawnFriend();
            }
		}

        if (coll.gameObject.layer == LayerMask.NameToLayer("FoodRange")) {
            goingUp = true;
            //closestFood = coll.gameObject;
            foods.Add(coll.gameObject);
            coll.gameObject.GetComponentInParent<FoodController>().index = foods.Count - 1;
		}
	}


    void OnTriggerExit(Collider coll) {

        if (coll.gameObject.layer == LayerMask.NameToLayer("FoodRange")) {
            goingUp = false;
            closestFood = null;
		}

    }

    void spawnRipple() {

        int rand = Random.Range(0, 100);
        if (rand == 1) {
            Instantiate(foodRipple, new Vector2(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1)), Quaternion.identity);
        }

    }

    void checkFood() {
        float dis = 100f;

        foreach(GameObject food in foods) {

            if (food != null) {
                Vector2 foodPos = new Vector2(food.transform.position.x, food.transform.position.z);
                Vector2 koiPos = new Vector2(transform.position.x, transform.position.z); 
                float newDis = Vector2.Distance(foodPos, koiPos);
                if (newDis < dis && newDis < 3) {
                    closestFood = food;
                    dis = newDis;
                }
            }
        }
    }

    void debugLine() {
       // Vector2 foodPos = new Vector2(closestFood.transform.position.x, closestFood.transform.position.z);
        //Vector2 koiPos = new Vector2(transform.position.x, transform.position.z); 
        line.SetPosition(0, transform.position);
        line.SetPosition(1, closestFood.transform.position);
    }

}
