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

    private Map()
    {
    }

    public Map(int x, int y)
    {
        mapTiles = new Tiles[x,y];
        sizeX = x;
        sizeY = y;
    }

    public void FillMapData(int seed)
    {
        System.Random randomTiles = new System.Random(seed);
        System.Random randomRGB = new System.Random(seed);
        int maxRandom = Tiles.MAX_HEIGHT;

        InitTiles();
        FillMainDots(randomRGB);
        FillCorners();
        DrawTiles();
    }

    private void InitTiles()
    {
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                mapTiles[i, j] = new Tiles();
                mapTiles[i, j].SetSpriteToTile(spritePrefab, i, j);
            }
    }

    private void FillMainDots(System.Random randomRGB)
    {
        for (int i = 1; i < sizeX - 1; i += 2)
            for (int j = 1; j < sizeY - 1; j += 2)
            {
                mapTiles[i, j].SetHeight(ChooseRGB(randomRGB.Next(0, 2)));
                FillAround(i, j);
            }
    }

    private void FillAround(int indX, int indY) 
    {
        for (int i = indX - 1; i < indX + 2; i++)
        {
            mapTiles[i, indY - 1].height += mapTiles[indX, indY].height.Normalized();
            mapTiles[i, indY + 1].height += mapTiles[indX, indY].height.Normalized();
        }
        mapTiles[indX - 1, indY].height += mapTiles[indX, indY].height.Normalized();
        mapTiles[indX + 1, indY].height += mapTiles[indX, indY].height.Normalized();
    }

    private void FillCorners()
    {
        mapTiles[0, 0].height += mapTiles[1,1].height.Normalized();
        mapTiles[sizeX-1, 0].height += mapTiles[sizeX-2, 1].height.Normalized();
        mapTiles[0, sizeY-1].height += mapTiles[1, sizeY-2].height.Normalized();
        mapTiles[sizeX-1, sizeY-1].height += mapTiles[sizeX-2, sizeY-2].height.Normalized();
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
        HeightRGB hR = new HeightRGB(4, 0, 0);
        HeightRGB hG = new HeightRGB(0, 4, 0);
        HeightRGB hB = new HeightRGB(0, 0, 4);
        List<HeightRGB> hList = new List<HeightRGB>();
        hList.Add(hR);
        hList.Add(hG);
        hList.Add(hB);
        return hList[index];
    }
}
