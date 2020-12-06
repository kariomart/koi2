using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse_3D : MonoBehaviour {

    public float speed;
    public float accel;
    public float deaccel;
    public float maxSpeed;
    public float defMaxSpeed;

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
    float bloomAmt = .5f;
    public int foodCounter;
    public int friendCounter = 16;
    public int firstFriend;

    Rigidbody rb;
    Vector3 dir;
    Vector3 mouseDir;
    float foodRange = 100f;
    public bool traveling;
    public GameObject randomFood;


    public int ambientTimer = 0;
    int gameStartedTimer;
	int ambientModeLockout = 480;

    public AudioSource droneSource;
    float desiredDroneVolume;

    public Material defaultMat;
    public Material koiOverlay;

    public Transform mouseDeciple;
    float mouseTimer;
    float droneTime = 120;



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
        //getRandomFood();
        getClosestFood();
        
        defMaxSpeed = maxSpeed;
    }
  
  // Update is called once per frame
  void FixedUpdate () {


        if (!GameMaster.me.flowMode) {
            checkAmbientMode();
        }
        ambientDrone();


        pos = new Vector2(transform.position.x, transform.position.z);

        waterSounds();
        if (foods.Count > 2) {
            checkFood();
        }

        Vector3 oldMouseDir = mouseDir;

        mouseDir = (mouseDeciple.position - transform.position).normalized;
        mouseDir.y=.51f;

        if (Input.GetMouseButton(0)) {
            speed+=accel;
            if (mouseTimer < droneTime){
                mouseTimer ++;
            }
        } else {
            speed-=deaccel;
            if (mouseTimer > 0) {
                mouseTimer --;
            }
        }

        maxSpeed = Mathf.MoveTowards(maxSpeed, defMaxSpeed, .00002f);

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
        reticle.position = mouseDeciple.position;

        AudioManager.Instance.updateDebug();

        if (GameMaster.me.debug) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                GameMaster.me.spawnFriend();
            }

            if (Input.GetKeyDown(KeyCode.A)) {
                GameMaster.me.ambientMode = !GameMaster.me.ambientMode;
            }
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

       gameStartedTimer++;
    }


    void checkAmbientMode() {
        if (speed == 0) {
            ambientTimer ++;
        }

        // if (ambientTimer > 1000 && !GameMaster.me.ambientMode) {
        //     GameMaster.me.ambientMode = true;
        // }

        if (speed == 0 && gameStartedTimer>ambientModeLockout && ambientTimer > 180) {
            GameMaster.me.ambientMode = true;
            reticle.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0)) {
            ambientTimer=0;
            GameMaster.me.ambientMode = false;
            reticle.gameObject.SetActive(true);
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

        float minVolume = .1f;
        float maxVol = .5f;
        
        
        float vol = Mathf.Lerp(minVolume, maxVol, mouseTimer/droneTime);
        if (GameMaster.me.ambientMode) {
            vol+= 0.001f;
            vol = Mathf.Clamp(vol, maxVol, minVolume);
        }
        droneSource.volume = vol;
        float panPosition = pos.x - cam.transform.position.x;
        panPosition = AudioManager.RemapFloat(panPosition, -3f, 3f, -.4f, .4f);
        droneSource.panStereo = panPosition;
    }

    public void changeMaterial() {
        GetComponent<ParticleSystemRenderer>().material = koiOverlay;
    }


    void OnTriggerEnter(Collider coll) {

        if (!GameMaster.me.flowMode) {
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

                effects.addBloom(bloomAmt);
                foodSpawner.spawnAFood();
                maxSpeed += 0.0035f;
                if (friends.Count == 0 && foodCounter >= firstFriend) {
                    GameMaster.me.spawnFriend();
                } else {
                    GameMaster.me.tryToSpawnFriend();
                }

                if (foodCounter != 10 && foodCounter % 10 == 0) {
                    GameMaster.me.spawnFriend();
                }
            }
		}
        Destroy(coll.gameObject);

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

    public void RemoveFriend() {
        KoiFriend koiFriend = friends[friends.Count-1];
        koiFriend.particles.Stop();
        GameObject koiFriendObj = friends[friends.Count-1].gameObject;
        Destroy(koiFriendObj, 1);
        friends.RemoveAt(friends.Count-1);
    }

    void spawnRipple() {

        GameObject r = Instantiate(foodRipple, new Vector3(pos.x, 0.51f, pos.y), Quaternion.identity);
        r.transform.SetParent(GameMaster.me.VFX);

    }

    public void spawnRipple(Vector3 position) {

        GameObject r = Instantiate(foodRipple, new Vector3(position.x, 0.51f, position.z), Quaternion.identity);
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
