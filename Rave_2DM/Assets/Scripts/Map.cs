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

    private int MAX_DEFAULT_TEMPERATURE = (int) TemperatureLevel.G4_BEST;

    List<LandscapeCode> baseLandscapeList = new List<LandscapeCode>();
    List<LandscapeCode> snowLandscapeList = new List<LandscapeCode>();


    private float orthogonalRatio;
    private float diagonalRatio;
    private int rareHumidityConst;
    private int R5Ratio;


    public Sprite mainDotSprite;

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
        CreateBaseLandscape(R5Ratio);
        CreateSnowLandscape();
        InitTiles();
        FillLandscapeAdnTemperature(randomRGB, tempCurve);
        FillWaterValues(seed);
    }

    private void CreateSnowLandscape()
    {
        snowLandscapeList.Clear();

        LandscapeCode hR0 = new LandscapeCode((int)HeightLevel.R0_DEEP_OCEAN, 0, 0);
        LandscapeCode hR2 = new LandscapeCode((int)HeightLevel.R2_OCEAN, 0, 0);

        snowLandscapeList.Add(hR0);
        snowLandscapeList.Add(hR2);
    }

    private void CreateBaseLandscape(int countR5)
    {
        baseLandscapeList.Clear();

        LandscapeCode hR0 = new LandscapeCode((int)HeightLevel.R0_DEEP_OCEAN, 0, 0);
        LandscapeCode hR4 = new LandscapeCode((int)HeightLevel.R4_PLAIN, 0, 0);
        LandscapeCode hR5 = new LandscapeCode((int)HeightLevel.R5_HILLS, 0, 0);

        baseLandscapeList.Add(hR0);
        baseLandscapeList.Add(hR4);
        for (int i = 0; i < countR5; i++)
            baseLandscapeList.Add(hR5);
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
        temp = tempCurve.Evaluate((float)y / SizeY) * (MAX_DEFAULT_TEMPERATURE + tempAddRatio);
        temp = Mathf.Round(temp);

        int R = (int)mTiles[ListIndex(x, y)].R;
        int G = (int)temp;
        int B = (int)mTiles[ListIndex(x, y)].B;

        mTiles[ListIndex(x, y)].SetHeight(R, G, B);
    }

    private void FillLandscapeAdnTemperature(System.Random randomRGB, AnimationCurve tempCurve)
    {
        LandscapeCode h;
        for (int i = 1; i < SizeX; i += 2)
            for (int j = 1; j < SizeY; j += 2)
            {
                if (j < 3 || j > SizeY - 3)
                    h = snowLandscapeList[randomRGB.Next(0, snowLandscapeList.Count)];
                else
                    h = baseLandscapeList[randomRGB.Next(0, baseLandscapeList.Count)];

                mTiles[ListIndex(i, j)].SetHeight(h);
                FillTemp(tempCurve, i, j, randomRGB.Next(-1, 2));
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

            if (item.X % 2 != 0 && item.Y % 2 != 0)
                ti.Building.GetComponent<SpriteRenderer>().sprite = mainDotSprite;

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

    public void SetSprites(Dictionary<HeightLevel, Sprite> groundTiles, Dictionary<int, Sprite> coastList)
    {
        // Set sprites by Height R
        foreach (var item in mTiles)
        {
            HeightLevel R = item.R;
            if (groundTiles.ContainsKey(R))
            {
                item.tileSprite = groundTiles[R];
            }

            if (item.R == HeightLevel.R3_COAST)
            {
                int countWater = CountOrtoTiles(item.X, item.Y);
                item.tileSprite = coastList[countWater];
                RotateCoast();
            }
        }


    }

    public  void RotateCoast()
    {
        // flip x,y 
        // x - left, right
        // y - up, down
    }

    private int CountOrtoTiles(int x, int y)
    {
        int res = 0;
        int xLeft, xRight, yUp, yDown;

        xLeft = x - 1;
        xRight = x + 1;
        if (xLeft < 0)
            xLeft = SizeX - 1;
        if (xRight > SizeX - 1)
            xRight = 0;

        if (mTiles[ListIndex(xLeft, y)].R < HeightLevel.R3_COAST)
            res++;
        if (mTiles[ListIndex(xRight, y)].R < HeightLevel.R3_COAST)
            res++;
        if (mTiles[ListIndex(x, y-1)].R < HeightLevel.R3_COAST)
            res++;
        if (mTiles[ListIndex(x, y+1)].R < HeightLevel.R3_COAST)
            res++;

        return res;
    }

    public void SetSprites(Dictionary<TemperatureLevel, Sprite> landTempTiles)
    {
        //Set Sprite by Temp
        foreach (var item in mTiles)
        {
            TemperatureLevel G = item.G;
            if (landTempTiles.ContainsKey(G))
                item.landscapeModificator = landTempTiles[G];
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