using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour {

	public GameObject noteFood;
	public GameObject chordFood;
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
		int rand;
		GameObject f;

		for (int i = 0; i < numOfFood; i++) {
			rand = Random.Range(0,5); 
			if (rand==1) {
				f = Instantiate (chordFood, new Vector3(Random.Range (-foodRange, foodRange), .51f, Random.Range (-foodRange, foodRange)), Quaternion.identity);
			} else {
				f = Instantiate (noteFood, new Vector3(Random.Range (-foodRange, foodRange), .51f, Random.Range (-foodRange, foodRange)), Quaternion.identity);
			}

			f.transform.parent = foodParent.transform;
			foods.Add(f);
		}
	}

}
