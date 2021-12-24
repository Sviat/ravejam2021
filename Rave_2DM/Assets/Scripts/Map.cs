using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public static int sizeX { get; private set; }
    public static int sizeY { get; private set; }
    public SpriteRenderer spritePrefab;
    private Tile[,] mapTiles;
    private Transform parent;
   
    List<HeightRGB> baseHeigthList = new List<HeightRGB>();
    private float lineRatio = 1.65f;
    private float diagonalRatio = 2.5f;
    private int RareHumidityConst = 1;
    private int countR5;

    private Map()
    {
        
    }

    public Map(int x, int y, int countR5)
    {
        mapTiles = new Tile[x, y];
        sizeX = x;
        sizeY = y;
        this.countR5 = countR5;
    }

    private void CreateHeightRGBList(int countR5)
    {
        int def = HeightRGB.DEFAULT_HEIGHT;
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

    public void FillMapData(int seed, Transform parent, AnimationCurve tempCurve)
    {
        System.Random randomRGB = new System.Random(seed);
        this.parent = parent;
        CreateHeightRGBList(countR5);
        InitTilesTemperature();
        FillHeightValues(randomRGB);
        FillTempValues(tempCurve);
        FillWaterValues(seed);
    }

    private void FillWaterValues(int seed)
    {
        System.Random randomW = new System.Random(seed);
        HeightValues R;
        TempValues G;

        WaterValues B;

        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                R = mapTiles[i, j].height.R;
                G = mapTiles[i, j].height.G;
                mapTiles[i, j].height.B = RG_to_B(R, G, randomW);
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

    private void FillTempValues(AnimationCurve tempCurve)
    {
        float temp;
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                temp = tempCurve.Evaluate((float) j / sizeY) * HeightRGB.MAX_TEMP;
                temp = Mathf.Round(temp);
                mapTiles[i, j].SetHeight((int)mapTiles[i, j].height.R, (int) temp, (int)mapTiles[i, j].height.B);
            }
    }

    private void InitTilesTemperature()
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                mapTiles[i, j] = new Tile();
                mapTiles[i, j].SetSpriteToTile(spritePrefab, i, j, parent);
            }
    }

    private void FillHeightValues(System.Random randomRGB) 
    {
        for (int i = 1; i < sizeX; i += 2)
            for (int j = 1; j < sizeY; j += 2)
            {
                mapTiles[i, j].SetHeight(baseHeigthList[randomRGB.Next(0, baseHeigthList.Count)]);
                FillAroundHeight(i, j);
            }
    }

    private void FillAroundHeight(int x, int y)
    {
        bool stopFlag = false;
        for (int i = x - 1; i <= x + 1 && !stopFlag; i++)
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i == sizeX)
                {
                    i = 0;
                    stopFlag = true;
                }
                if (!(i == x && j == y))
                {
                    if (i == x || j == y)
                        mapTiles[i, j].height += mapTiles[x, y].height / lineRatio;
                    else
                        mapTiles[i, j].height += mapTiles[x, y].height / diagonalRatio;
                }
            }
    }

    public void DrawTiles(bool r, bool g, bool b)
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {   
                mapTiles[i, j].DrawTile(r, g, b);
            }
    }
}

