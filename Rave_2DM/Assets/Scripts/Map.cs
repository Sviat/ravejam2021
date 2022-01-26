using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Map
{
    public static int SizeX { get; private set; }
    public static int SizeY { get; private set; }

    [SerializeField] private List<Tile> mTiles;
    [SerializeField] private List<TileGameObject> tInfos;

    private int MAX_DEFAULT_TEMPERATURE = (int)TemperatureLevel.G4_BEST;
    private List<LandscapeCode> baseLandscapeList = new List<LandscapeCode>();
    private List<LandscapeCode> snowLandscapeList = new List<LandscapeCode>();


    private float orthogonalRatio;
    private float diagonalRatio;
    private int rareHumidityConst;
    private int[] R2R4R6Ratio;
    private int mainTileRatio;
    private System.Random levelRandom;
    private List<Point> aroundTiles = new List<Point>();
    public Sprite mainDotSprite;

    private Map()
    {

    }

    public Map(int x, int y, int[] R2R4R6Ratio, int seed, AnimationCurve tempCurve, float orthogonalRatio,
                float diagonalRatio, int rareHumidityConst, int mainTileRatio)
    {
        SizeX = x;
        SizeY = y;
        mTiles = new List<Tile>();
        tInfos = new List<TileGameObject>();
        this.R2R4R6Ratio = R2R4R6Ratio;
        this.orthogonalRatio = orthogonalRatio;
        this.diagonalRatio = diagonalRatio;
        this.rareHumidityConst = rareHumidityConst;
        this.mainTileRatio = mainTileRatio;
        levelRandom = new System.Random(seed);
        FillMapData(seed, tempCurve);
    }

    public List<Point> FindTileByLandscape(LandscapeCode height)
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

        CreateBaseLandscape(R2R4R6Ratio);
        CreateSnowLandscape();
        InitTiles();
        FillLandscape();
        ///FillTemperature(tempCurve);
        //FillWaterValues();
    }

    private void InitTiles()
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                mTiles.Add(new Tile(i, j));
            }
    }

    private void FillLandscape()
    {
        LandscapeCode h;
        // Fill Core Tiles
        for (int i = 1; i < SizeX; i += 2)
        {
            for (int j = 1; j < SizeY; j += 2)
            {
                h = baseLandscapeList[levelRandom.Next(0, baseLandscapeList.Count)];
                mTiles[ListIndex(i, j)].SetLandscape(h);
            }
        }

        if (mainTileRatio > 1)
        {
            for (int i = 1; i < SizeX; i += 2)
            {
                for (int j = 3; j < SizeY - 3; j += 2)
                {
                    var coreNeighbors = GetAroundAllTiles(i, j, 2).Concat(GetAroundOrtoTiles(i, j, 4)).ToList();
                    Debug.Log($"CoreNeib = {coreNeighbors.Count}");
                    
                    for (int mainCount = 0; mainCount < mainTileRatio; mainCount++)
                        coreNeighbors.Add(new Point(i, j));

                    var coreTileChosen = RandomChoice(coreNeighbors, levelRandom);
                    h = mTiles[ListIndex(coreTileChosen.x, coreTileChosen.y)].Height;

                    mTiles[ListIndex(i, j)].SetLandscape(h);
                    //FillTemperature(tempCurve, i, j, levelRandom.Next(-1, 2));
                    //FillAroundHeight(i, j);
                }
            }
        }

        for (int i = 1; i < SizeX; i += 2)
        {
            for (int j = 1; j < SizeY - 3; j += 2)
            {
                FillQuadByPatterns(new Point(i, j));
            }
        }


        for (int i = 1; i < SizeX; i += 2)
        {
            for (int j = 1; j < SizeY - 3; j += 2)
            {
                FillOrthogonalHeight(i, j);
            }
        }


        /* for (int i = 0; i < SizeX; i += 2)
         {
             for (int j = 2; j < SizeY - 2; j += 2)
             {
                 FillDiagonalHeight(i, j);
             }
         }*/



        // Fill Polar Cap (R0, R2)
        /*  for (int i = 1; i < SizeX; i += 2)
          {
              int j = 1;
              h = snowLandscapeList[levelRandom.Next(0, snowLandscapeList.Count)];
              mTiles[ListIndex(i, j)].SetLandscape(h);
              h = snowLandscapeList[levelRandom.Next(0, snowLandscapeList.Count)];
              mTiles[ListIndex(i, SizeY - j - 1)].SetLandscape(h);

          }*/
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
        var orthoList = GetAroundOrtoTiles(x, y);
        if (mTiles[ListIndex(x, y)].R == HeightLevel.R2_OCEAN)
        {
            // if before haven't been filled by R2 (R0 / R2 / R3)
            foreach (var item in orthoList)
            {
                if (mTiles[ListIndex(item)].R != HeightLevel.R2_OCEAN && mTiles[ListIndex(item)].R != HeightLevel.R0_DEEP_OCEAN)
                    mTiles[ListIndex(item)].SetHeight(HeightLevel.R3_COAST);
            }
        }

        if (mTiles[ListIndex(x, y)].R == HeightLevel.R6_MOUNTAINS)
        {
            // if before haven't been filled 
            foreach (var item in orthoList)
            {
                if (mTiles[ListIndex(item)].R == HeightLevel.R_UNDEFINED)

                    mTiles[ListIndex(item)].SetHeight(HeightLevel.R5_HILLS);
            }
        }
    }

    private void FillQuadByPatterns(Point baseTile)
    {
        var coreTilesPoints = GetFourCoreTiles(baseTile);
        int r2Count = CountTilesByHeight(coreTilesPoints, HeightLevel.R2_OCEAN);
        int r4Count = CountTilesByHeight(coreTilesPoints, HeightLevel.R4_PLAIN);
        int r6Count = CountTilesByHeight(coreTilesPoints, HeightLevel.R6_MOUNTAINS);
        List<HeightLevel> orthoHeights = new List<HeightLevel>();
        List<HeightLevel> diagonalHeights = new List<HeightLevel>(); ;

        // x4 Pattern 
        if (r2Count == 4) // OTY
        {
            orthoHeights = new List<HeightLevel>()
            {
                HeightLevel.R0_DEEP_OCEAN,
                HeightLevel.R2_OCEAN
            };
            diagonalHeights = new List<HeightLevel>() { HeightLevel.R0_DEEP_OCEAN };
        }
        else if (r4Count == 4) // HM
        {
            orthoHeights = new List<HeightLevel>() { HeightLevel.R4_PLAIN };
            diagonalHeights = new List<HeightLevel>()
            {
                HeightLevel.R3_COAST,
                HeightLevel.R4_PLAIN, HeightLevel.R4_PLAIN, HeightLevel.R4_PLAIN, // Add R4 Here to change Ratio
                HeightLevel.R5_HILLS
            };
        }
        else if (r6Count == 4) // KPU
        {
            orthoHeights = new List<HeightLevel>()
            {
                HeightLevel.R8_EVEREST,
                HeightLevel.R6_MOUNTAINS, HeightLevel.R6_MOUNTAINS, HeightLevel.R6_MOUNTAINS, HeightLevel.R6_MOUNTAINS, HeightLevel.R6_MOUNTAINS,
                HeightLevel.R5_HILLS, HeightLevel.R5_HILLS, HeightLevel.R5_HILLS, HeightLevel.R5_HILLS
            };
            diagonalHeights = new List<HeightLevel>() { HeightLevel.R8_EVEREST };
        }

        if (orthoHeights.Count != 0 && diagonalHeights.Count != 0)
        {
            SetFullPattern(coreTilesPoints, diagonalHeights, orthoHeights);
            return;
        }
        //x4 Pattern END


        if (r2Count == 3)
        {
            
        }
    }

    private List<Point> GetOrthoInQuad(Point diagCoord)
    {
        return GetAroundOrtoTiles(diagCoord.x, diagCoord.y);
    }

    private Point GetDiagonalInQuad(Point[] coreTilesPoints)
    {
        Point diagCoord = new Point(0, 0);
        foreach (var item in coreTilesPoints)
            diagCoord += item;
        diagCoord /= coreTilesPoints.Length;
        return diagCoord;
    }

    private void SetFullPattern(Point[] coreTilesPoints, List<HeightLevel> diagonalHeights, List<HeightLevel> orthoHeights)
    {
        Point diagPoint = GetDiagonalInQuad(coreTilesPoints);
        mTiles[ListIndex(diagPoint)].SetHeight(RandomChoice(diagonalHeights, levelRandom));

        foreach (var orthoPoint in GetOrthoInQuad(diagPoint))
        {
            mTiles[ListIndex(orthoPoint)].SetHeight(RandomChoice(orthoHeights, levelRandom));
        }
    }

    private int CountTilesByHeight(Point[] tilePoints, HeightLevel height)
    {
        int result = 0;
        foreach (var item in tilePoints)
        {
            if (mTiles[ListIndex(item)].R == height)
                result++;
        }
        return result;
    }

    private Point[] GetFourCoreTiles(Point p)
    {
        var coreTiles = new Point[4];
        coreTiles[0] = new Point(p.x, p.y);
        coreTiles[1] = new Point(p.x, p.y + 2);
        coreTiles[2] = new Point(p.x + 2, p.y + 2);
        coreTiles[3] = new Point(p.x + 2, p.y);
        return coreTiles;
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

    private void CreateBaseLandscape(int[] R2R4R6Ratio)
    {
        baseLandscapeList.Clear();

        LandscapeCode hR2 = new LandscapeCode((int)HeightLevel.R2_OCEAN, 0, 0);
        LandscapeCode hR4 = new LandscapeCode((int)HeightLevel.R4_PLAIN, 0, 0);
        LandscapeCode hR6 = new LandscapeCode((int)HeightLevel.R6_MOUNTAINS, 0, 0);

        for (int i = 0; i < R2R4R6Ratio[0]; i++)
        {
            baseLandscapeList.Add(hR2);
        }

        for (int i = 0; i < R2R4R6Ratio[1]; i++)
        {
            baseLandscapeList.Add(hR4);
        }

        for (int i = 0; i < R2R4R6Ratio[2]; i++)
        {
            baseLandscapeList.Add(hR6);
        }
    }

    private void FillWaterValues()
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
                B = RG_to_B(R, G, levelRandom);
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
        return y + SizeY * WrapX(x);
    }
    private int ListIndex(Point p)
    {
        return p.y + SizeY * WrapX(p.x);
    }

    public void CreateGameObjects(Transform prefab, Transform parent, bool isCopy)
    {
        Transform go;
        TileGameObject ti;

        foreach (var item in mTiles)
        {
            go = MonoBehaviour.Instantiate(prefab, new Vector2(item.X, item.Y), Quaternion.identity, parent);
            ti = go.gameObject.GetComponent<TileGameObject>();
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

    public void SetCoasts(Sprite[] coastListSprites)
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 1; j < SizeY - 1; j++)
            {
                if (mTiles[ListIndex(i, j)].R > HeightLevel.R3_COAST)
                {
                    aroundTiles = GetAroundOrtoTiles(i, j);
                    for (int indx = 0; indx < aroundTiles.Count; indx++)
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
        
        result.Add(new Point(xLeft, y)); // 0
        if ((y + distance) < SizeY)
        {
            result.Add(new Point(x, y + distance)); // 1 
        }

        result.Add(new Point(xRight, y)); //2
        if ((y - distance) >= 0)
        {
            result.Add(new Point(x, y - distance)); //3
        }
        
        

        return result;
    }
    public List<Point> GetAroundOrtoTiles(Point p, int distance = 1)
    {
        return GetAroundOrtoTiles(p.x, p.y, distance);
    }

    public List<Point> GetAroundDiagonalTiles(int x, int y, int distance = 1)
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
        return GetAroundOrtoTiles(x, y, distance).Concat(GetAroundDiagonalTiles(x, y, distance)).ToList();
    }

    public void SetCoastTileInfo(Sprite sprite, int x, int y, int indexPosition)
    {
        List<TileGameObject> findedTInfos = new List<TileGameObject>();
        findedTInfos = FindTInfos(x, y);
        if (findedTInfos != null)
        {
            foreach (var item in findedTInfos)
            {
                item.SetCoast(indexPosition, sprite);
            }
        }
    }

    public List<TileGameObject> FindTInfos(int x, int y)
    {
        List<TileGameObject> result = new List<TileGameObject>();
        foreach (var item in tInfos)
        {
            if (item.tile.X == x && item.tile.Y == y)
                result.Add(item);
        }
        return result;
    }

}