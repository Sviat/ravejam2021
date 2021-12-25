using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapCreator : MonoBehaviour
{
    [SerializeField] private  int sizeX, sizeY;
    [SerializeField] private int mapCreatorSeed;
    [SerializeField] private SpriteRenderer spritePrefab;
    [SerializeField] private Sprite[] tileSprites;
    private Map map;
    private bool isCreated;
    private Transform mapCenter, mapLeft, mapRight;
    private readonly string mapName = "MapTiles";

    [SerializeField] private int countR5;
    [SerializeField] private AnimationCurve tempCurve;
    private (bool, bool, bool) RGB = (true, true, true);
    private bool rgbChanged = false;

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
            System.DateTime time =System.DateTime.Now;
            if (isCreated)
                DeleteMap();    
            CreateMapTileObject();
            map = new Map(x, y, countR5)
            {
                spritePrefab = spritePrefab
            };
            map.FillMapData(seed, mapCenter, tempCurve);
            map.DrawTiles(RGB.Item1, RGB.Item2, RGB.Item3);
            //CopyMap();
            CenterMap();
            isCreated = true;
            Debug.Log($"Map create time = {System.DateTime.Now - time}");
        }
        else
            Debug.Log("Wrong sizeX, sizeY");
    }

    private void CreateMapTileObject()
    {
        mapCenter = new GameObject().transform;
        mapCenter.SetParent(transform);
        mapCenter.name = mapName;
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
        mapRight = Instantiate(mapCenter, new Vector2(mapCenter.position.x + sizeX, mapCenter.position.y), Quaternion.identity);
        mapLeft = Instantiate(mapCenter, new Vector2(mapCenter.position.x - sizeX, mapCenter.position.y), Quaternion.identity, transform);
        mapRight.SetParent(transform);
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RGB.Item1 = !RGB.Item1;
            rgbChanged = true;

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RGB.Item2 = !RGB.Item2;
            rgbChanged = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RGB.Item3 = !RGB.Item3;
            rgbChanged = true;
        }

        if (rgbChanged)
        {
            map.DrawTiles(RGB.Item1, RGB.Item2, RGB.Item3);
            rgbChanged = false;
        }
    }

    private void CenterMap()
    {
        transform.position =new Vector3(((float)sizeX) / -2.0f, ((float)sizeY) / -2.0f, transform.position.z);
    }
}
