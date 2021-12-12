using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private int sizeX, sizeY;
    [SerializeField]
    private int mapCreatorSeed;
    private Map map;
    private bool isCreated;
    [SerializeField]
    private SpriteRenderer spritePrefab;

    private void Start()
    {
        isCreated = false;
    }

    private void CreateMap(int x, int y, int seed)
    {
        if (!isCreated)
        {
            map = new Map(x, y);
            map.spritePrefab = spritePrefab;
            map.FillMapData(seed);
            isCreated = true;
        }
        else
            Debug.Log("Map is already created. Delete first");
    }

    private void DeleteMap()
    {
        if (isCreated)
        {
            for (int i = 0; i < map.sizeX; i++)
                for (int j = 0; j < map.sizeY; j++)
                    Destroy(map.mapTiles[i, j].tileGameObject.gameObject);
            isCreated = false;
        }
        else
            Debug.Log("Map doesn't exist. Create first");
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CreateMap(sizeX, sizeY, mapCreatorSeed);
        if (Input.GetKeyDown(KeyCode.D))
            DeleteMap();
    }

}
