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

    List<HeightRGB> baseHeigthList = new List<HeightRGB>();

    private float lineRatio = 1.65f;
    private float diagonalRatio = 2.5f;
    private int RareHumidityConst = 1;
    private int countR5;

    private Map()
    {

    }

    public Map(int x, int y, int countR5, int seed, AnimationCurve tempCurve)
    {
        SizeX = x;
        SizeY = y;
        mTiles = new List<Tile>();
        tInfos = new List<TileInfo>();
        this.countR5 = countR5;
        FillMapData(seed, tempCurve);
    }

    public List<(int, int)> FindTileByRGB(HeightRGB height)
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
        CreateHeightRGBList(countR5);
        InitTiles();
        FillHeightTempValues(randomRGB, tempCurve);
        FillWaterValues(seed);
    }

    private void CreateHeightRGBList(int countR5)
    {
        baseHeigthList.Clear();

        HeightRGB hR0 = new HeightRGB((int)HeightValues.R0_DEEP_OCEAN, 0, 0);
        HeightRGB hR4 = new HeightRGB((int)HeightValues.R4_PLAIN, 0, 0);
        HeightRGB hR5 = new HeightRGB((int)HeightValues.R5_HILLS, 0, 0);
        HeightRGB hR6 = new HeightRGB((int)HeightValues.R6_MOUNTAINS, 0, 0);

        baseHeigthList.Add(hR0);
        baseHeigthList.Add(hR4);
        baseHeigthList.Add(hR6);

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
        HeightValues R;
        TempValues G;
        WaterValues B;

        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                int mIndex = ListIndex(i, j);
                R = mTiles[mIndex].R;
                G = mTiles[mIndex].G;
                B = RG_to_B(R, G, randomW);
                mTiles[mIndex].SetHeight(new HeightRGB(R, G, B));
            }
    }

    private WaterValues RG_to_B(HeightValues R, TempValues G, System.Random random)
    {

        int min, max;
        int waterLevel;
        int currentChance;
        WaterValues resB;

        currentChance = random.Next(0, 100);
        if (currentChance <= RareHumidityConst)
        {
            if (random.Next(0, 2) == 0)
                resB = WaterValues.B0_OCEAN_OF_WATER;
            else resB = WaterValues.B8_DESERT;

            return resB;
        }

        min = (int)R < (int)G ? (int)R : (int)G;
        max = (int)R > (int)G ? (int)R + 1 : (int)G + 1;
        waterLevel = random.Next(min, max);

        if (waterLevel == 1)
            waterLevel = 2;
        if (waterLevel == 7)
            waterLevel = 6;
        resB = (WaterValues)waterLevel;

        return resB;
    }

    private void FillTemp(AnimationCurve tempCurve, int x, int y, int tempAddRatio)
    {
        float temp;
        temp = tempCurve.Evaluate((float)y / SizeY) * ((int)HeightRGB.DEFAULT_TEMP + tempAddRatio);
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
                HeightRGB h = baseHeigthList[randomRGB.Next(0, baseHeigthList.Count)];

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
                        ratio = lineRatio;
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

    public void SetSprites(Dictionary<HeightValues, Sprite> groundTiles)
    {
        // Set sprites by Height R
        foreach (var item in mTiles)
        {
            HeightValues R = item.R;
            if (groundTiles.ContainsKey(R))
            {
                item.tileSprite = groundTiles[R];
            }
        }
    }
    public void SetSprites(Dictionary<TempValues, Sprite> landTempTiles)
    {
        //Set Sprite by Temp
        foreach (var item in mTiles)
        {
            TempValues G = item.G;
            if ((landTempTiles.ContainsKey(G) && item.R == HeightValues.R4_PLAIN))
                item.tileSprite = landTempTiles[G];
            if (item.G > TempValues.G6_HEAT && item.R< HeightValues.R6_MOUNTAINS)
                item.tileSprite = landTempTiles[TempValues.G6_HEAT];
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