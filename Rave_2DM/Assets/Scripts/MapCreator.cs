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
    [SerializeField]
    private Transform mapTiles;


    private void Start()
    {
        isCreated = false;
    }

    private bool CheckSize()
    {
        bool ret = true;
        if (sizeX % 2 != 0)
            ret = false;
        if (sizeY % 2 == 0)
            ret = false;
        return ret;
    }

    private void CreateMap(int x, int y, int seed)
    {
        if (CheckSize())
        {
            if (isCreated)
                DeleteMap();

            map = new Map(x, y);
            map.spritePrefab = spritePrefab;
            map.FillMapData(seed, mapTiles);
            isCreated = true;
            mapCreatorSeed++;
        }
        else
            Debug.Log("Wrong sizeX, sizeY");

    }

    private void DeleteMap()
    {
        if (isCreated)
        {
            for (int i = 0; i < map.sizeX; i++)
                for (int j = 0; j < map.sizeY; j++)
                    Destroy(map.mapTiles[i, j].tileGameObject.gameObject);
            isCreated = false;
            // удалять также копии карт
        }
        else
            Debug.Log("Map doesn't exist. Create first");
    }

    private void CopyMap()
    {
        Instantiate(mapTiles, new Vector2(mapTiles.position.x + sizeX, mapTiles.position.y), Quaternion.identity, transform);
        Instantiate(mapTiles, new Vector2(mapTiles.position.x - sizeX, mapTiles.position.y), Quaternion.identity, transform);
        // Записывать в переменные, чтобы потом удалять
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CreateMap(sizeX, sizeY, mapCreatorSeed);
        if (Input.GetKeyDown(KeyCode.D))
            DeleteMap();
        if (Input.GetKeyDown(KeyCode.M))
            CopyMap();

    }

}
