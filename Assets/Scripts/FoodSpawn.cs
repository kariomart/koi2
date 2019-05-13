using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour {

	public GameObject noteFood;
	public GameObject chordFood;
	public int numOfFood;
	public int foodRange;
	GameObject foodParent;
	public List<GameObject> foods = new List<GameObject>();

	// Use this for initialization
	void Start () {

		SpawnFood ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnFood() {

		foodParent = new GameObject("Foods");
		int rand;
		GameObject f;

		for (int i = 0; i < numOfFood; i++) {
			rand = Random.Range(0,4); 
			if (rand==1) {
				f = Instantiate (chordFood, new Vector3(Random.Range (-foodRange, foodRange), .51f, Random.Range (-foodRange, foodRange)), Quaternion.identity);
			} else {
				f = Instantiate (noteFood, new Vector3(Random.Range (-foodRange, foodRange), .51f, Random.Range (-foodRange, foodRange)), Quaternion.identity);
			}

			f.transform.parent = foodParent.transform;
			foods.Add(f);
		}
	}

	public void spawnAFood() {

		int rand1 = Random.Range(0,2);
		int rand2 = Random.Range(0,2);
		int rand3 = Random.Range(0,4);
		int x;
		int y;
		Vector3 p = GameMaster.me.player.transform.position;
		GameObject f;

		if (rand1==0) {
			x=9+Random.Range(0,5);
		} else {
			x=-9-Random.Range(0,5);
		}

		if (rand2==0) {
			y=5+Random.Range(0,5);
		} else {
			y=-5-Random.Range(0,5);
		}

		if (rand3 == 1) {
			f = Instantiate (chordFood, new Vector3(p.x+x, .51f,p.z+y), Quaternion.identity);
		} else {
			f = Instantiate (noteFood, new Vector3(p.x+x, .51f, p.z+y), Quaternion.identity);
		}

		//Debug.Log("spawned food @ " + x + " " + y);
		f.transform.parent = foodParent.transform;
		foods.Add(f);

	}

}
