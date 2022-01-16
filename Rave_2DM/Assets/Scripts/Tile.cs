using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile
{
    [SerializeField] private LandscapeCode landCode;
    [SerializeField] private int x, y;

    public Sprite tileSprite;
    public Sprite landscapeModificator;
    public Sprite resourceSprite;
   

    [SerializeField] private BuildingSlot buildingSlot;
    private Buildings BuiltGameObject = null;
    private bool canBuild;
    private TradeGoodsTypes good = TradeGoodsTypes.Null;
    [Range(0, 1)]
    private float remainingResourses = 1;


    public int X 
    {
        get
        {
            return x;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }
    }

    public Tile(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public Tile(int _x, int _y, LandscapeCode _h)
    {
        x = _x;
        y = _y;
        landCode = _h;
    }

    public void SetLandscape (int heightR, int heightG, int heightB)
    {
        landCode = new LandscapeCode(heightR, heightG, heightB);
    }

    public void SetLandscape(LandscapeCode height)
    {
        this.landCode = height;
    }

    public void SetHeight(HeightLevel height)
    {
        this.landCode.R = height;
    }

    public void AddHeight(LandscapeCode addValue)
    {
        landCode += addValue;
    }

    public LandscapeCode Height
    {
        get
        {
            return landCode;
        }
    }

    public HeightLevel R
    {
        get { return landCode.R; }
    }

    public TemperatureLevel G
    {
        get { return landCode.G; }
    }

    public HumidityLevel B
    {
        get { return landCode.B; }
    }

}
