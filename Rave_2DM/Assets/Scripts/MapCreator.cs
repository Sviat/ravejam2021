using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    public int sizeX, sizeY;
    [SerializeField]
    private TerrainTiles[] tilesPrefabs;
    private TerrainTiles[,] mapTiles;
    private System.Random randForTiles;
    private int tileSize; // Now doesnt use (init in Start), but may be later
    [SerializeField]
    public int mapCreatorSeed;
    private bool isCreated;
       
    private void Start()
    {
        tileSize = 1;
        isCreated = false;
    }

    private void CreateMap(int x, int y, int seed)
    {
        if (!isCreated)
        {
            System.DateTime timeStart = System.DateTime.Now;
            int maxRandom = tilesPrefabs.Length;

            mapTiles = new TerrainTiles[x, y];
            randForTiles = new System.Random(seed);

            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    int tileIndex = randForTiles.Next(0, maxRandom);
                    mapTiles[i, j] = Instantiate(tilesPrefabs[tileIndex], new Vector2(i * tileSize, j * tileSize), 
                        Quaternion.identity, transform);
                }
            Debug.Log($"Time for Map create = {System.DateTime.Now - timeStart}");
            isCreated = true;
        }
        else
        {
            Debug.Log("Map is already created. Delete first");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CreateMap(sizeX, sizeY, mapCreatorSeed);
        if (Input.GetKeyDown(KeyCode.D))
            DeleteMap();
    }

    private void DeleteMap()
    {
        if (isCreated && mapTiles.Length >0)
        {
            foreach (var e in mapTiles)
                Destroy(e.gameObject);
            isCreated = false;
        }
        else
            Debug.Log("There is no map. Create Map first");
    }
}
