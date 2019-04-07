using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour {

	public GameObject food;
	public int numOfFood;
	public int foodRange;
	public List<GameObject> foods = new List<GameObject>();

	// Use this for initialization
	void Start () {

		SpawnFood ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnFood() {

		GameObject foodParent = new GameObject("Foods");

		for (int i = 0; i < numOfFood; i++) {

			GameObject f = Instantiate (food, new Vector3(Random.Range (-foodRange, foodRange), .51f, Random.Range (-foodRange, foodRange)), Quaternion.identity);
			f.transform.parent = foodParent.transform;
			foods.Add(f);

		}
	}

}
