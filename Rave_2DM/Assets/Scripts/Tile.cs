using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile
{
    [SerializeField] private LandscapeCode height;
    [SerializeField] private int x, y;

    public Sprite tileSprite;
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
        height = _h;
    }

    public void SetHeight (int heightR, int heightG, int heightB)
    {
        height = new LandscapeCode(heightR, heightG, heightB);
    }

    public void SetHeight(LandscapeCode height)
    {
        this.height = height;
    }

    public void AddHeight(LandscapeCode addValue)
    {
        height += addValue;
    }

    public LandscapeCode Height
    {
        get
        {
            return height;
        }
    }

    public HeightLevel R
    {
        get { return height.R; }
    }

    public TemperatureLevel G
    {
        get { return height.G; }
    }

    public HumidityLevel B
    {
        get { return height.B; }
    }

}
