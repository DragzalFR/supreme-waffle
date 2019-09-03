using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public GameObject tile;

    static public int width = 10;
    static public int lenght = 10;
    private GameObject[,] grid = new GameObject[width, lenght];

	// Use this for initialization
    // Cree les tuiles et les nommes "Tile[$X, $Z]"
	void Start () {

        // Create the board
		for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < lenght; z++)
            {
                GameObject gridTile = Instantiate(tile) as GameObject;
                gridTile.transform.position = new Vector3(
                    gridTile.transform.position.x + x + 0.5f,
                    gridTile.transform.position.y,
                    gridTile.transform.position.z + z + 0.5f);
                gridTile.name = "Tile [" + x + ", " + z + "]";
                //var color = gridTile.GetComponent<Renderer>().material.GetColor("_Color");
                //color.a = 0.2f;
                //gridTile.GetComponent<Renderer>().material.SetColor("_Color", color);
                grid[x, z] = gridTile;
            }
        }

        Destroy(GameObject.Find("Tile"));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
