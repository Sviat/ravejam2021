using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public static int sizeX { get; private set; }
    public static int sizeY { get; private set; }
    public SpriteRenderer spritePrefab;
    public Tiles[,] mapTiles;
    private Transform parent;

    List<HeightRGB> baseHeigthList = new List<HeightRGB>();

    private Map()
    {
        CreateHeightRGBList();
    }

    public Map(int x, int y)
    {
        mapTiles = new Tiles[x, y];
        sizeX = x;
        sizeY = y;
        CreateHeightRGBList();
    }

    private void CreateHeightRGBList()
    {
        int def = HeightRGB.DEFAULT_HEIGHT;
        HeightRGB hR = new HeightRGB(def, 0, 0);
        HeightRGB hG = new HeightRGB(0, def, 0);
        HeightRGB hB = new HeightRGB(0, 0, def);
        baseHeigthList.Add(hR);
        baseHeigthList.Add(hG);
        baseHeigthList.Add(hB);
    }

    public void FillMapData(int seed, Transform parent)
    {
        this.parent = parent;
        System.Random randomRGB = new System.Random(seed);
        InitTiles();
        FillMapTiles(randomRGB);
        DrawTiles();
    }

    private void InitTiles()
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                mapTiles[i, j] = new Tiles();
            }
    }

    private void FillMapTiles(System.Random randomRGB) 
    {
        for (int i = 1; i < sizeX; i += 2)
            for (int j = 1; j < sizeY; j += 2)
            {
                mapTiles[i, j].SetHeight(baseHeigthList[randomRGB.Next(0, baseHeigthList.Count)]);
                FillAround(i, j);
            }
    }

    private void FillAround(int x, int y)
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
                        mapTiles[i, j].height += mapTiles[x, y].height.Normalized2X();
                    else
                        mapTiles[i, j].height += mapTiles[x, y].height.Normalized();
                }
            }
    }

    private void DrawTiles()
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                mapTiles[i, j].SetSpriteToTile(spritePrefab, i, j, parent);
                mapTiles[i, j].DrawTile();
            }
    }
}

