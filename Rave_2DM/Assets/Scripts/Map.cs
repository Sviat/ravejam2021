using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum HeightValues {R0, R2, R3, R4, R5, R6, R8}
enum TempValues {G0, G2, G3, G4, G5, G6, G8}
enum WaterValues {B0, B2, B3, B4, B5, B6, B8}

public class Map 
{
    public int sizeX { get; }
    public int sizeY { get; }
    public SpriteRenderer spritePrefab;
    public Tiles[,] mapTiles;
    private Transform parent;

    private Map()
    {
    }

    public Map(int x, int y)
    {
        mapTiles = new Tiles[x,y];
        sizeX = x;
        sizeY = y;
    }

    public void FillMapData(int seed, Transform parent)
    {
        this.parent = parent;
        System.Random randomTiles = new System.Random(seed);
        System.Random randomRGB = new System.Random(seed);
        int maxRandom = HeightRGB.MAX_HEIGHT;

        InitTiles();
        FillMainDots(randomRGB);
        FillFirst();
        DrawTiles();
    }

    private void InitTiles()
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                mapTiles[i, j] = new Tiles();
                mapTiles[i, j].SetSpriteToTile(spritePrefab, i, j, parent);
            }
    }

    private void FillMainDots(System.Random randomRGB)
    {
        for (int i = 1; i < sizeX; i += 2)
            for (int j = 1; j < sizeY; j += 2)
            {
                mapTiles[i, j].SetHeight(ChooseRGB(randomRGB.Next(0, 3)));
                FillAround(i, j);
            } 
    }

    private void FillAround(int indX, int indY) 
    { 
        for (int j = indY - 1; j < indY + 2; j++)
        {
            mapTiles[indX-1, j].height += mapTiles[indX, indY].height.Normalized2X();
            if ((indX+1) < sizeX)
            mapTiles[indX+1, j].height += mapTiles[indX, indY].height.Normalized2X();
        }
        mapTiles[indX, indY-1].height += mapTiles[indX, indY].height.Normalized();
        mapTiles[indX, indY+1].height += mapTiles[indX, indY].height.Normalized();
    }

    private void FillFirst()
    {
        for (int j = 1; j < sizeY; j += 2)
        {
            mapTiles[0, j-1].height += mapTiles[sizeX - 1, j].height.Normalized();
            mapTiles[0, j].height += mapTiles[sizeX - 1, j].height.Normalized2X();
            mapTiles[0, j+1].height += mapTiles[sizeX - 1, j].height.Normalized();
        }
    }

    private void DrawTiles()
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                mapTiles[i, j].DrawTile();
            }
    }

    private HeightRGB ChooseRGB(int index) // Refactor later 
    {
        int def = HeightRGB.DEFAULT_HEIGHT;

        HeightRGB hR = new HeightRGB(def, 0, 0);
        HeightRGB hG = new HeightRGB(0, def, 0);
        HeightRGB hB = new HeightRGB(0, 0, def);
        List<HeightRGB> hList = new List<HeightRGB>();
        hList.Add(hR);
        hList.Add(hG);
        hList.Add(hB);
        return hList[index];
    }
}
