using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Map
{
    public static int SizeX { get; private set; }
    public static int SizeY { get; private set; }
    [SerializeField] private List<Tile> mTiles;
    [SerializeField] private List<TileInfo> tInfos;

    private int DEFAULT_TEMPERATURE = (int) TemperatureLevel.G4_BEST;

    List<LandscapeCode> baseHeigthList = new List<LandscapeCode>();
    private float orthogonalRatio;
    private float diagonalRatio;
    private int rareHumidityConst;
    private int R5Ratio;


    private Map()
    {

    }

    public Map(int x, int y, int R5Ratio, int seed, AnimationCurve tempCurve, float orthogonalRatio, float diagonalRatio, int rareHumidityConst)
    {
        SizeX = x;
        SizeY = y;
        mTiles = new List<Tile>();
        tInfos = new List<TileInfo>();
        this.R5Ratio = R5Ratio;
        this.orthogonalRatio = orthogonalRatio;
        this.diagonalRatio = diagonalRatio;
        this.rareHumidityConst = rareHumidityConst;
        FillMapData(seed, tempCurve);
    }

    public List<(int, int)> FindTileByRGB(LandscapeCode height)
    {
        List<(int, int)> indexXY = new List<(int, int)>();
        foreach (var e in mTiles)
        {
            if (e.Height == height)
                indexXY.Add((e.X, e.Y));
        }
        return indexXY;
    }

    public void FillMapData(int seed, AnimationCurve tempCurve)
    {
        System.Random randomRGB = new System.Random(seed);
        CreateHeightRGBList(R5Ratio);
        InitTiles();
        FillHeightTempValues(randomRGB, tempCurve);
        FillWaterValues(seed);
    }

    private void CreateHeightRGBList(int countR5)
    {
        baseHeigthList.Clear();

        LandscapeCode hR0 = new LandscapeCode((int)HeightLevel.R0_DEEP_OCEAN, 0, 0);
        LandscapeCode hR4 = new LandscapeCode((int)HeightLevel.R4_PLAIN, 0, 0);
        LandscapeCode hR5 = new LandscapeCode((int)HeightLevel.R5_HILLS, 0, 0);
        LandscapeCode hR6 = new LandscapeCode((int)HeightLevel.R6_MOUNTAINS, 0, 0);

        baseHeigthList.Add(hR0);
        baseHeigthList.Add(hR4);
        //baseHeigthList.Add(hR6);

        for (int i = 0; i < countR5; i++)
            baseHeigthList.Add(hR5);
    }

    private void InitTiles()
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                mTiles.Add(new Tile(i, j));
            }
    }

    private void FillWaterValues(int seed)
    {
        System.Random randomW = new System.Random(seed);
        HeightLevel R;
        TemperatureLevel G;
        HumidityLevel B;

        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                int mIndex = ListIndex(i, j);
                R = mTiles[mIndex].R;
                G = mTiles[mIndex].G;
                B = RG_to_B(R, G, randomW);
                mTiles[mIndex].SetHeight(new LandscapeCode(R, G, B));
            }
    }

    private HumidityLevel RG_to_B(HeightLevel R, TemperatureLevel G, System.Random random)
    {

        int min, max;
        int waterLevel;
        int currentChance;
        HumidityLevel resB;

        currentChance = random.Next(0, 100);
        if (currentChance <= rareHumidityConst)
        {
            if (random.Next(0, 2) == 0)
                resB = HumidityLevel.B0_OCEAN_OF_WATER;
            else resB = HumidityLevel.B8_DESERT;

            return resB;
        }

        min = (int)R < (int)G ? (int)R : (int)G;
        max = (int)R > (int)G ? (int)R + 1 : (int)G + 1;
        waterLevel = random.Next(min, max);

        if (waterLevel == 1)
            waterLevel = 2;
        if (waterLevel == 7)
            waterLevel = 6;
        resB = (HumidityLevel)waterLevel;

        return resB;
    }

    private void FillTemp(AnimationCurve tempCurve, int x, int y, int tempAddRatio)
    {
        float temp;
        temp = tempCurve.Evaluate((float)y / SizeY) * (DEFAULT_TEMPERATURE + tempAddRatio);
        temp = Mathf.Round(temp);

        int R = (int)mTiles[ListIndex(x, y)].R;
        int G = (int)temp;
        int B = (int)mTiles[ListIndex(x, y)].B;

        mTiles[ListIndex(x, y)].SetHeight(R, G, B);
    }

    private void FillHeightTempValues(System.Random randomRGB, AnimationCurve tempCurve)
    {
        for (int i = 1; i < SizeX; i += 2)
            for (int j = 1; j < SizeY; j += 2)
            {
                LandscapeCode h = baseHeigthList[randomRGB.Next(0, baseHeigthList.Count)];

                mTiles[ListIndex(i, j)].SetHeight(h);
                FillTemp(tempCurve, i, j, randomRGB.Next(0, 2));
                FillAroundHeight(i, j);
            }
    }

    private void FillAroundHeight(int x, int y)
    {
        bool stopFlag = false;
        float ratio;
        for (int i = x - 1; i <= x + 1 && !stopFlag; i++)
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i == SizeX)
                {
                    i = 0;
                    stopFlag = true;
                }

                if (!(i == x && j == y))
                {
                    if (i == x || j == y)
                        ratio = orthogonalRatio;
                    else
                        ratio = diagonalRatio;
                   
                    mTiles[ListIndex(i, j)].AddHeight(mTiles[ListIndex(x, y)].Height / ratio);
                    
                }
            }
    }

    private int ListIndex(int x, int y)
    {
        return y + SizeY * x;
    }

    public void CreateGameObjects(Transform prefab, Transform parent, bool isCopy)
    {
        Transform go;
        TileInfo ti;

        foreach (var item in mTiles)
        {
            go = MonoBehaviour.Instantiate(prefab, new Vector2(item.X, item.Y), Quaternion.identity, parent);
            ti = go.gameObject.GetComponent<TileInfo>();
            ti.SetTileInfo(item, isCopy);
            //ti.DrawTile(true, true, true);
            tInfos.Add(ti);
        }
    }

    public void DrawTilesAll(bool r, bool g, bool b)
    {
        foreach (var item in tInfos)
        {
            item.DrawTile(r, g, b);
        }
    }

    public void SetSprites(Dictionary<HeightLevel, Sprite> groundTiles)
    {
        // Set sprites by Height R
        foreach (var item in mTiles)
        {
            HeightLevel R = item.R;
            if (groundTiles.ContainsKey(R))
            {
                item.tileSprite = groundTiles[R];
            }
        }
    }
    public void SetSprites(Dictionary<TemperatureLevel, Sprite> landTempTiles)
    {
        //Set Sprite by Temp
        foreach (var item in mTiles)
        {
            TemperatureLevel G = item.G;
            if ((landTempTiles.ContainsKey(G) && item.R == HeightLevel.R4_PLAIN))
                item.tileSprite = landTempTiles[G];
            if (item.G > TemperatureLevel.G6_HEAT && item.R< HeightLevel.R6_MOUNTAINS)
                item.tileSprite = landTempTiles[TemperatureLevel.G6_HEAT];
        }
    }
    public void SetSpritesToObjects()
    {
        foreach (var item in tInfos)
        {
            if (item.GetTileSprite())
            {
                item.SetSpriteToTile();
            }
        }
    }
}