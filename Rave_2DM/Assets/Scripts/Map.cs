using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public struct Point : IEquatable<Point>
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    bool IEquatable<Point>.Equals(Point other)
    {
        return x==other.x && y == other.y;
    }
}

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
    private int R4Ratio;
    private int mainTileRatio;

    List<Point> aroundTiles= new List<Point>();

    public Sprite mainDotSprite;

    private Map()
    {

    }

    public Map(int x, int y, int R5Ratio, int seed, AnimationCurve tempCurve, float orthogonalRatio, 
                float diagonalRatio, int rareHumidityConst, int mainTileRatio)
    {
        SizeX = x;
        SizeY = y;
        mTiles = new List<Tile>();
        tInfos = new List<TileInfo>();
        this.R4Ratio = R5Ratio;
        this.orthogonalRatio = orthogonalRatio;
        this.diagonalRatio = diagonalRatio;
        this.rareHumidityConst = rareHumidityConst;
        this.mainTileRatio = mainTileRatio;
        FillMapData(seed, tempCurve);
    }

    public List<Point> FindTileByRGB(LandscapeCode height)
    {
        List<Point> indexXY = new List<Point>();
        foreach (var e in mTiles)
        {
            if (e.Height == height)
                indexXY.Add(new Point(e.X, e.Y));
        }
        return indexXY;
    }

    public void FillMapData(int seed, AnimationCurve tempCurve)
    {
        System.Random randomRGB = new System.Random(seed);
        CreateBaseLandscape(R4Ratio);
        CreateSnowLandscape();
        InitTiles();
        FillLandscape(randomRGB);
        ///FillTemperature(randomRGB, tempCurve);
        FillWaterValues(randomRGB);
    }

    private void InitTiles()
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                mTiles.Add(new Tile(i, j));
            }
    }

    private void FillLandscape(System.Random randomRGB)
    {
        LandscapeCode h;

        for (int i = 1; i < SizeX; i += 2)
        {
            for (int j = 1; j < SizeY; j += 2)
            {
                h = baseLandscapeList[randomRGB.Next(0, baseLandscapeList.Count)];
                mTiles[ListIndex(i, j)].SetLandscape(h);
            }
        }

        for (int i = 1; i < SizeX; i += 2)
        {
            for (int j = 3; j < SizeY - 3; j += 2)
            {
                var coreNeighbors = GetAroundAllTiles(i, j, 2).Concat(GetAroundOrtoTiles(i, j, 4)).ToList();

                for (int mainCount = 0; mainCount < mainTileRatio; mainCount++)
                    coreNeighbors.Add(new Point(i, j));

                var coreTileChosen = RandomChoice(coreNeighbors, randomRGB);
                h = mTiles[ListIndex(coreTileChosen.x, coreTileChosen.y)].Height;

                mTiles[ListIndex(i, j)].SetLandscape(h);
                //FillTemperature(tempCurve, i, j, randomRGB.Next(-1, 2));
                //FillAroundHeight(i, j);
            }
        }

        for (int i = 1; i < SizeX; i += 2)
        {
            int j = 1;
            h = snowLandscapeList[randomRGB.Next(0, snowLandscapeList.Count)];
            mTiles[ListIndex(i, j)].SetLandscape(h);
            h = snowLandscapeList[randomRGB.Next(0, snowLandscapeList.Count)];
            mTiles[ListIndex(i, SizeY - j - 1)].SetLandscape(h);

        }

                for (int i = 0; i < SizeX; i += 2)
        {
            for (int j = 2; j < SizeY - 2 ; j += 2)
            {
                FillDiagonalHeight(i, j);
            }
        }

        for (int i = 0; i < SizeX; i += 1)
        {
            for (int j = 1; j < SizeY - 1; j += 1)
            {
                if ((i + j) % 2 == 0)
                    continue;
                FillOrthogonalHeight(i, j);
            }
        }

    }

    public T RandomChoice<T>(List<T> bag, System.Random random)
    {
        return bag[random.Next(0, bag.Count)];
    }

    private void FillTemperature(AnimationCurve tempCurve, int x, int y, int tempAddRatio)
    {
        float temp;
        temp = tempCurve.Evaluate((float)y / SizeY) * (MAX_DEFAULT_TEMPERATURE + tempAddRatio);
        temp = Mathf.Round(temp);

        int R = (int)mTiles[ListIndex(x, y)].R;
        int G = (int)temp;
        int B = (int)mTiles[ListIndex(x, y)].B;

        mTiles[ListIndex(x, y)].SetLandscape(R, G, B);
    }

    private void FillOrthogonalHeight(int x, int y)
    {
        var tiles = GetAroundOrtoTiles(x, y);
        int resultR = 0;
        int tile1;
        int tile2;

        if (x % 2 == 0)
        {
            tile1 = (int)mTiles[ListIndex(WrapX(x - 1), y)].R;
            tile2 = (int)mTiles[ListIndex(WrapX(x + 1), y)].R;
        }
        else
        {
            tile1 = (int)mTiles[ListIndex(x, y - 1)].R;
            tile2 = (int)mTiles[ListIndex(x, y + 1)].R;
        }
        HeightLevel result;

        if (tile1 == tile2)
        {
            resultR = tile1 + tile2 - 4;
        } else
        {

            if (Math.Abs(tile1 - tile2) == 1)
                resultR = Math.Abs(tile1 - 4) < Math.Abs(tile2 - 4) ? tile1 : tile2;
            else
                resultR = (tile1 + tile2) / 2; //??
            
        }

        result = LandscapeCode.HeightLevelFromInt(resultR);
      /*  if (resultR == 1 || resultR == -1)
            result = HeightLevel.R4_PLAIN;
        else
        {
            result = (HeightLevel)(resultR + 4);
        }*/
        mTiles[ListIndex(x, y)].SetHeight(result);
    }

    private static int WrapX(int x)
    {
        return (x + SizeX) % SizeX;
    }

    private void FillDiagonalHeight(int x, int y)
    {
        var tiles = GetAroundDiagonalTiles(x, y);
        int resultR = -12;

        foreach (var item in tiles)
        {
            resultR += (int)mTiles[ListIndex(item.x, item.y)].R;
        }

        if (resultR == 3 || resultR == 5)
            resultR = 4;
        
        mTiles[ListIndex(x, y)].SetHeight(LandscapeCode.HeightLevelFromInt(resultR));
    }

    private void FillAroundHeight(int x, int y)
    {
        float ratio;
        aroundTiles = GetAroundAllTiles(x, y);
       
        for (int i = 0; i < aroundTiles.Count; i++)
        {
            int x1, y1;
            x1 = aroundTiles[i].x;
            y1 = aroundTiles[i].y;

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

    private void CreateBaseLandscape(int countR4)
    {
        baseLandscapeList.Clear();

        LandscapeCode hR0 = new LandscapeCode((int)HeightLevel.R0_DEEP_OCEAN, 0, 0);
        LandscapeCode hR2 = new LandscapeCode((int)HeightLevel.R2_OCEAN, 0, 0);
        LandscapeCode hR3 = new LandscapeCode((int)HeightLevel.R3_COAST, 0, 0);
        LandscapeCode hR4 = new LandscapeCode((int)HeightLevel.R4_PLAIN, 0, 0);
        LandscapeCode hR5 = new LandscapeCode((int)HeightLevel.R5_HILLS, 0, 0);

        //baseLandscapeList.Add(hR0);
        //baseLandscapeList.Add(hR2);
        baseLandscapeList.Add(hR3);
        for (int i = 0; i < countR4; i++)
        {
            baseLandscapeList.Add(hR4);
        }
        baseLandscapeList.Add(hR5);
    }

     private void FillWaterValues(System.Random randomW)
    {
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
                mTiles[mIndex].SetLandscape(new LandscapeCode(R, G, B));
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
                    aroundTiles = GetAroundOrtoTiles(i, j);
                    for (int indx=0; indx < aroundTiles.Count; indx++)
                    {
                        if (mTiles[ListIndex(aroundTiles[indx].x, aroundTiles[indx].y)].R < HeightLevel.R4_PLAIN)
                            SetCoastTileInfo(coastListSprites[indx], i, j, indx);
                    }
                }
            }
    }

    public List<Point> GetAroundOrtoTiles(int x, int y, int distance = 1)
    {
        List<Point> result = new List<Point>();
        int xLeft = (x - distance + SizeX) % SizeX;
        int xRight = (x + distance) % SizeX;
        if ((y + distance) < SizeY )
        {
            result.Add(new Point (x, y + distance));
        }
        if ((y - distance) >= 0)
        {
            result.Add(new Point(x, y - distance));
        }
        result.Add(new Point(xLeft, y));
        result.Add(new Point(xRight, y));

        return result;
    }

    public List<Point> GetAroundDiagonalTiles(int x, int y, int distance =1)
    {
        List<Point> result = new List<Point>();
        int xLeft = (x - distance + SizeX) % SizeX;
        int xRight = (x + distance) % SizeX;
        if ((y + distance) < SizeY)
        {
            result.Add(new Point(xLeft, y + distance));
            result.Add(new Point(xRight, y + distance));
        }
        if ((y - distance) >= 0)
        {
            result.Add(new Point(xLeft, y - distance));
            result.Add(new Point(xRight, y - distance));
        }
        
        return result;
    }

    public List<Point> GetAroundAllTiles(int x, int y, int distance = 1)
    {
        return GetAroundOrtoTiles(x, y, distance).Concat(GetAroundDiagonalTiles(x,y,distance)).ToList();
        
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