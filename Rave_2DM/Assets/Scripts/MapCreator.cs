using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapCreator : MonoBehaviour
{
    [SerializeField] private  int sizeX, sizeY;
    [SerializeField] private int mapCreatorSeed;
    [SerializeField] private Transform spritePrefab;

    // Sprites Info
    [SerializeField] private Sprite[] groundSprites; // land, ocean, sea
    Dictionary<HeightValues, Sprite> groundTiles;
    [SerializeField] private Sprite[] landTempSprites; // land, ocean, sea
    Dictionary<TempValues, Sprite> landTempTiles;
    // End Sprites Info

    [SerializeField] private Map map;
    private Transform mapCenter, mapLeft, mapRight;
    private readonly string mapName = "MapTiles";

    [SerializeField] private int countR5;
    [SerializeField] private AnimationCurve tempCurve;
    private (bool, bool, bool) RGB = (true, true, true);
    private bool rgbChanged = false;

    private void Start()
    {
        // Add sprite values to Dictionary (refactor later)
        groundTiles = new Dictionary<HeightValues, Sprite>();
        landTempTiles = new Dictionary<TempValues, Sprite>();
        groundTiles.Add(HeightValues.R0_DEEP_OCEAN, groundSprites[0]);
        groundTiles.Add(HeightValues.R2_OCEAN, groundSprites[1]);
        groundTiles.Add(HeightValues.R4_PLAIN, groundSprites[2]);
        groundTiles.Add(HeightValues.R5_HILLS, groundSprites[3]);
        groundTiles.Add(HeightValues.R6_MOUNTAINS, groundSprites[4]);
        groundTiles.Add(HeightValues.R8_EVEREST, groundSprites[5]);

        landTempTiles.Add(TempValues.G5_WARM, landTempSprites[0]);
        landTempTiles.Add(TempValues.G6_HEAT, landTempSprites[1]);
        // End Add sprites;


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
            if (map != null)
                DeleteMap();    

            CreateMapTileObject(out mapCenter);
            //CreateMapTileObject(out mapLeft);
            //CreateMapTileObject(out mapRight);


            map = new Map(x, y, countR5, seed, tempCurve);
            map.CreateGameObjects(spritePrefab, mapCenter, isCopy: false);
           // map.CreateGameObjects(spritePrefab, mapLeft, isCopy: true);
           // map.CreateGameObjects(spritePrefab, mapRight, isCopy: true);

            mapCenter.position = new Vector2(0.5f, 0);
            //mapLeft.position = new Vector2(mapCenter.position.x - sizeX, mapCenter.position.y);
           // mapRight.position = new Vector2(mapCenter.position.x + sizeX, mapCenter.position.y);

            map.SetSprites(groundTiles);
            map.SetSprites(landTempTiles);

            map.SetSpritesToObjects();


            CenterMap();
            Debug.Log($"Map create time = {System.DateTime.Now - time}");
        }
        else
            Debug.Log("Wrong sizeX, sizeY");
    }

    private void CreateMapTileObject(out Transform mapTransform)
    {       
        mapTransform = new GameObject().transform;
        mapTransform.SetParent(transform);
        mapTransform.name = mapName;
    }

    private void DeleteMap()
    {
        if (map!=null)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            map = null;
        }
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateMap(sizeX, sizeY, mapCreatorSeed);
            mapCreatorSeed++;
            rgbChanged = false;
            RGB.Item1 = RGB.Item2 = RGB.Item3 = true;
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
            map.DrawTilesAll(RGB.Item1, RGB.Item2, RGB.Item3);
            rgbChanged = false;
        }
    }

    private void CenterMap()
    {
        transform.position =new Vector3(((float)sizeX) / -2.0f, ((float)sizeY) / -2.0f, transform.position.z);
    }
}
