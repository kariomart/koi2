using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse_3D : MonoBehaviour {

    public float moveSpeed = 0.004f;
    public float xSpeed;
    public float ySpeed;
    public float speed;
    public float accel = 0.0001f;
    public float deaccel = 0.0001f;
    public float maxSpeed = 0.001f;

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
    public FoodSpawn foodSpawner;

    public List<KoiFriend> friends = new List<KoiFriend>();
    float bloomAmt = .25f;
    public int foodCounter;
    public int friendCounter = 16;

    Rigidbody rb;
    Vector3 dir;
    Vector3 mouseDir;
    float foodRange = 100f;
    public bool traveling;
    public GameObject randomFood;


    public int ambientTimer = 0;

    public AudioSource droneSource;
    float desiredDroneVolume;

    public Material defaultMat;
    public Material koiOverlay;


    // Use this for initialization
    void Start () {
        
        reticle = GameObject.Find("reticle").transform;
        sphere = GetComponent<SphereCollider>();
        rb=GetComponent<Rigidbody>();
        Cursor.visible = false;
        cam = Camera.main;
        waterSfx = GetComponent<AudioSource>();
        line = GetComponent<LineRenderer>();
        depthPercentage = 1;
        FX = GetComponent<ParticleSystem>();
        waterShader = water.GetComponent<Renderer>().material;
        defaultMat = GetComponent<ParticleSystemRenderer>().material;
        getRandomFood();
    }
  
  // Update is called once per frame
  void FixedUpdate () {

        checkAmbientMode();
        ambientDrone();



        pos = new Vector2(transform.position.x, transform.position.z);

        waterSounds();
        if (foods.Count > 2) {
            checkFood();
        }

        Vector3 oldMouseDir = mouseDir;

        mouseDir = (cam.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        mouseDir.y=.51f;

        if (Input.GetMouseButton(0)) {
            speed+=accel;
        } else {
            speed-=accel;
        }

        speed = Mathf.Clamp(speed, 0, maxSpeed);


        if (!GameMaster.me.ambientMode) {
            dir = mouseDir;
        } else {
            if (randomFood) {
                dir = (randomFood.transform.position - transform.position).normalized;
                speed=.035f*(.1f+Mathf.Abs(Mathf.Sin(Time.time+Random.Range(0f,.5f))));
                //speed*= .01f +Vector2.Distance(pos, new Vector2(randomFood.transform.position.x, randomFood.transform.position.z)/10f);
            } else {
                getClosestFood();
            }
        }

        transform.position = transform.position + dir * speed;
        transform.position = new Vector3(transform.position.x, .51f, transform.position.z);

        AudioManager.Instance.updateDebug();

        if (Input.GetKeyDown(KeyCode.Space)) {
            GameMaster.me.spawnFriend();
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            //AudioManager.Instance.scaleNum = AudioManager.Instance.scaleNum % AudioManager.Instance.scales.Length;

        }

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


    void checkAmbientMode() {
         ambientTimer ++;

        if (ambientTimer > 1000 && !GameMaster.me.ambientMode) {
            GameMaster.me.ambientMode = true;
        }

        if (Input.GetMouseButton(0)) {
            ambientTimer=0;
            GameMaster.me.ambientMode = false;
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

    void ambientDrone() {
        float maxVol = .25f;
        float minVolume = .05f;
        
        float vol = Mathf.Lerp(minVolume, maxVol, speed/maxSpeed);
        droneSource.volume = vol;
        float panPosition = pos.x - cam.transform.position.x;
        panPosition = AudioManager.RemapFloat(panPosition, -3f, 3f, -.4f, .4f);
        droneSource.panStereo = panPosition;
    }

    public void changeMaterial() {
        GetComponent<ParticleSystemRenderer>().material = koiOverlay;
    }


    void OnTriggerEnter(Collider coll) {

		if (coll.gameObject.layer == LayerMask.NameToLayer("Food")) {
            getClosestFood();
            AudioManager.Instance.PlayFoodSound();
            spawnRipple();
            foodCounter++;
            if (friends.Count > 0) {
                if (coll.gameObject.tag == "Chord") {
                    Debug.Log("Playing Chord");
                    GameMaster.me.StartCoroutine("playFriendChord");
                } else {
                    Debug.Log("Playing Note");
                    GameMaster.me.StartCoroutine("playFriendNotes");
                }
            }
            //AudioManager.Instance.FX.StartPumpingBloom();
            effects.addBloom(bloomAmt);
			Destroy(coll.gameObject);
            foodSpawner.spawnAFood();

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

        GameObject r = Instantiate(foodRipple, new Vector3(pos.x, 0.51f, pos.y), Quaternion.identity);
        r.transform.SetParent(GameMaster.me.VFX);

    }

    void checkFood() {
        float dis = 100;

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

    void getClosestFood() {
        float dis = foodRange;

         foreach(GameObject food in foodSpawner.foods) {

            if (food != null) {
                Vector2 foodPos = new Vector2(food.transform.position.x, food.transform.position.z);
                Vector2 koiPos = new Vector2(transform.position.x, transform.position.z); 
                float newDis = Vector2.Distance(foodPos, koiPos);
                if (newDis < dis) {
                    randomFood = food;
                    dis = newDis;
                }
            }
        }

    }

    void getRandomFood() {
        randomFood = foodSpawner.foods[Random.Range(0,foods.Count)];
    }

    void debugLine() {
       // Vector2 foodPos = new Vector2(closestFood.transform.position.x, closestFood.transform.position.z);
        //Vector2 koiPos = new Vector2(transform.position.x, transform.position.z); 
        line.SetPosition(0, transform.position);
        line.SetPosition(1, closestFood.transform.position);
    }

}
