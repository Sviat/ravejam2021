using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] private int sizeX, sizeY;
    [SerializeField] private int mapCreatorSeed;
    [SerializeField] private SpriteRenderer spritePrefab;
    [SerializeField] private Sprite[] tileSprites;
    private Map map;
    private bool isCreated;
    private Transform mapTiles, mapTilesLeft, mapTilesRight;
    private string mapTileName = "MapTiles";

    public int countR, countG, countB;

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
            map = new Map(x, y, countR, countG, countB);
            map.spritePrefab = spritePrefab;
            map.FillMapData(seed, mapTiles);
            map.DrawTiles(tileSprites);
            CopyMap();
            CenterMap();
            isCreated = true;
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
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            isCreated = false;
        }
        transform.position = new Vector3(0, 0, 0);
    }

    private void CopyMap()
    {
        mapTilesRight = Instantiate(mapTiles, new Vector2(mapTiles.position.x + sizeX, mapTiles.position.y), Quaternion.identity);
        mapTilesLeft = Instantiate(mapTiles, new Vector2(mapTiles.position.x - sizeX, mapTiles.position.y), Quaternion.identity, transform);
        mapTilesRight.SetParent(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateMap(sizeX, sizeY, mapCreatorSeed);
            mapCreatorSeed++;
        }
        if (Input.GetKeyDown(KeyCode.D))
            DeleteMap();
    }

    private void CenterMap()
    {
        transform.position =new Vector3(((float)sizeX) / -2.0f, ((float)sizeY) / -2.0f, transform.position.z);
    }
}
