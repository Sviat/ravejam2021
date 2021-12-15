using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] private int sizeX, sizeY;
    [SerializeField] private int mapCreatorSeed;
    [SerializeField] private SpriteRenderer spritePrefab;
    private Map map;
    private bool isCreated;
    private Transform mapTiles, mapTilesLeft, mapTilesRight;
    private string mapTileName = "MapTiles";

    private void Start()
    {
        isCreated = false;
        CreateMap(sizeX, sizeY, mapCreatorSeed);
    }

    private bool CheckSize()
    {
        return sizeX % 2 == 0 && sizeY % 2 != 0;
    }

    private void CreateMap(int x, int y, int seed)
    {
        if (CheckSize())
        {
            if (isCreated)
                DeleteMap();
            CreateMapTileObject();
            map = new Map(x, y);
            map.spritePrefab = spritePrefab;
            map.FillMapData(seed, mapTiles);
            CopyMap();
            isCreated = true;
            mapCreatorSeed++;
        }
        else
            Debug.Log("Wrong sizeX, sizeY");
    }

    private void CreateMapTileObject()
    {
        mapTiles = new GameObject().transform;
        mapTiles.SetParent(transform);
        mapTiles.name = mapTileName;
    }

    private void DeleteMap()
    {
        if (isCreated)
        {
            Destroy(mapTiles.gameObject);
            isCreated = false;
        }
        else
            Debug.Log("Map doesn't exist. Create first");
    }

    private void CopyMap()
    {
        mapTilesRight = Instantiate(mapTiles, new Vector2(mapTiles.position.x + sizeX, mapTiles.position.y), Quaternion.identity, mapTiles);
        mapTilesLeft = Instantiate(mapTiles, new Vector2(mapTiles.position.x - sizeX, mapTiles.position.y), Quaternion.identity, mapTiles);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CreateMap(sizeX, sizeY, mapCreatorSeed);
        if (Input.GetKeyDown(KeyCode.D))
            DeleteMap();
    }
}
