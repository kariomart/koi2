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


	// Use this for initialization
	void Start () {

		me = this;
		tiles = new GameObject("BG_TILES");
		//PlaceBackground(player.transform.position);
		
	}
	
	// Update is called once per frame
	void Update () {

	}

}
