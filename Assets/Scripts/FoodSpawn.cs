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
			rand = Random.Range(0,3); 
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

		GameObject food;
		GameObject spawnedFood; 
		int rand = Random.Range(0,4);
		int xRange = 20;
		int yRange = 12;
		int x;
		int y;

		if (rand == 1) {
			food = chordFood;
		} else {
			food = noteFood;
		}


		Vector3 p = GameMaster.me.player.transform.position;

		rand = Random.Range(1,3);

		if (rand == 1) {
			x=xRange+Random.Range(0,4);
			spawnedFood = Instantiate (food, new Vector3(p.x+x, .51f,p.z+Random.Range(-yRange, yRange)), Quaternion.identity);
			spawnedFood.transform.parent = foodParent.transform;
			foods.Add(spawnedFood);
		}

		rand = Random.Range(1,3);

		if (rand == 1) {
			x=-xRange-Random.Range(0,4);
			spawnedFood = Instantiate (food, new Vector3(p.x+x, .51f,p.z + Random.Range(-yRange, yRange)), Quaternion.identity);
			spawnedFood.transform.parent = foodParent.transform;
			foods.Add(spawnedFood);
		}

		rand = Random.Range(1,3);
		if (rand == 1) {
			y=yRange+Random.Range(0,4);
			spawnedFood = Instantiate (food, new Vector3(p.x + Random.Range(-xRange, xRange), .51f,p.z+y), Quaternion.identity);
			spawnedFood.transform.parent = foodParent.transform;
			foods.Add(spawnedFood);
		}

		rand = Random.Range(1,3);

		if (rand == 1) {
			y=-yRange-Random.Range(0,yRange);
			spawnedFood = Instantiate (food, new Vector3(p.x + Random.Range(-xRange, xRange), .51f,p.z+y), Quaternion.identity);
			spawnedFood.transform.parent = foodParent.transform;
			foods.Add(spawnedFood);
		}
	



		//Debug.Log("spawned food @ " + x + " " + y);

	}

}
