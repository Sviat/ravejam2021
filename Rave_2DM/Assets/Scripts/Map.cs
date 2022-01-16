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

    List<(int, int)> aroundTiles= new List<(int, int)>();

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
        FillLandscapeAndTemperature(randomRGB, tempCurve);
        FillWaterValues(seed);
    }

    private void InitTiles()
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                mTiles.Add(new Tile(i, j));
            }
    }

    private void FillLandscapeAndTemperature(System.Random randomRGB, AnimationCurve tempCurve)
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
                FillTemperature(tempCurve, i, j, randomRGB.Next(-1, 2));
                FillAroundHeight(i, j);
            }
    }

    private void FillTemperature(AnimationCurve tempCurve, int x, int y, int tempAddRatio)
    {
        float temp;
        temp = tempCurve.Evaluate((float)y / SizeY) * (MAX_DEFAULT_TEMPERATURE + tempAddRatio);
        temp = Mathf.Round(temp);

        int R = (int)mTiles[ListIndex(x, y)].R;
        int G = (int)temp;
        int B = (int)mTiles[ListIndex(x, y)].B;

        mTiles[ListIndex(x, y)].SetHeight(R, G, B);
    }

    private void FillAroundHeight(int x, int y)
    {
        float ratio;
        SetAroundAllTiles(x, y);
        for (int i = 0; i < aroundTiles.Count; i++)
        {
            int x1, y1;
            x1 = aroundTiles[i].Item1;
            y1 = aroundTiles[i].Item2;

            if (!(x1 == x && y1 == y))
            {
                if (x1 == x || y1 == y)
                    ratio = orthogonalRatio;
                else
                    ratio = diagonalRatio;

                mTiles[ListIndex(x1, y1)].AddHeight(mTiles[ListIndex(x, y)].Height / ratio);
            }
        }
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

    public void SetCoasts(Sprite [] coastListSprites)
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 1; j < SizeY - 1; j++)
            {
                if (mTiles[ListIndex(i, j)].R > HeightLevel.R3_COAST)
                {
                    SetAroundOrtoTiles(i, j);
                    for (int indx=0; indx < aroundTiles.Count; indx++)
                    {
                        if (mTiles[ListIndex(aroundTiles[indx].Item1, aroundTiles[indx].Item2)].R < HeightLevel.R4_PLAIN)
                            SetCoastTileInfo(coastListSprites[indx], i, j, indx);
                    }
                }
            }
    }

    public void SetAroundOrtoTiles(int x, int y)
    {
        int xLeft, xRight;
        if (x == 0)
            xLeft = SizeX - 1;
        else xLeft = x - 1;

        if (x == SizeX - 1)
            xRight = 0;
        else xRight = x + 1;

        aroundTiles.Clear();
        aroundTiles.Add((xLeft, y));
        aroundTiles.Add((xRight, y));
        aroundTiles.Add((x, y + 1));
        aroundTiles.Add((x, y - 1));
    }

    public void SetAroundDiagonalTiles(int x, int y)
    {
        int xLeft, xRight;
        if (x == 0)
            xLeft = SizeX - 1;
        else xLeft = x - 1;

        if (x == SizeX - 1)
            xRight = 0;
        else xRight = x + 1;

        aroundTiles.Clear();
        aroundTiles.Add((xLeft, y - 1));
        aroundTiles.Add((xLeft, y+1));
        aroundTiles.Add((xRight, y+1));
        aroundTiles.Add((xRight, y - 1));
    }

    public void SetAroundAllTiles(int x, int y)
    {
        int xLeft, xRight;
        if (x == 0)
            xLeft = SizeX - 1;
        else xLeft = x - 1;

        if (x == SizeX - 1)
            xRight = 0;
        else xRight = x + 1;

        aroundTiles.Clear();
        for (int j = y - 1; j <= y + 1; j++)
        {
            aroundTiles.Add((xLeft, j));
            if (x != j)
                aroundTiles.Add((x, j));
            aroundTiles.Add((xRight, j));
        }
    }

    public void SetCoastTileInfo(Sprite sprite, int x, int y, int indexPosition)
    {
        List<TileInfo> findedTInfos = new List<TileInfo>();
        findedTInfos = FindTInfos(x, y);
        if (findedTInfos != null)
        {
            foreach (var item in findedTInfos)
            {
                item.SetCoast(indexPosition, sprite);
            }
        }
    }

    public List<TileInfo> FindTInfos(int x, int y)
    {
        List<TileInfo> result = new List<TileInfo>();
        foreach (var item in tInfos)
        {
            if (item.tileSetInMap.X == x && item.tileSetInMap.Y == y)
                result.Add(item);
        }

        return result;
    }
    
}