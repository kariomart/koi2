using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator me;
	public GameObject bgTile;
	public GameObject bg;
	public float tileSize;
	public int xTiles;
	public int yTiles;

	public FollowMouse player;
	GameObject tiles;
	List<Vector2> tileOrigins = new List<Vector2>();
	Vector2 tileOrigin;


	// Use this for initialization
	void Start () {

		me = this;
		tiles = new GameObject("BG_TILES");
		//PlaceBackground(player.transform.position);
		
	}
	
	// Update is called once per frame
	void Update () {

	}




	void checkPlayer() {


		foreach(Vector2 pos in tileOrigins) {

			float halfWidth = tileSize * xTiles / 2;
			float maxXPos = pos.x + halfWidth;

			if (Mathf.Abs(player.transform.position.x) > maxXPos) {
				Vector2 newOrigin = new Vector2(tileOrigin.x + halfWidth * 2, tileOrigin.y);
				PlaceBackground(newOrigin);
			} 
		}
	}

	void PlaceBackground(Vector2 pos) {

		Instantiate(bg, pos, Quaternion.identity);
		tileOrigins.Add(pos);


	}
}
