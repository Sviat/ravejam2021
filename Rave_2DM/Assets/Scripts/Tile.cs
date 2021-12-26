using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile
{
    [SerializeField] private HeightRGB height;
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

    public Tile(int _x, int _y, HeightRGB _h)
    {
        x = _x;
        y = _y;
        height = _h;
    }

    public void SetHeight (int heightR, int heightG, int heightB)
    {
        height = new HeightRGB(heightR, heightG, heightB);
    }

    public void SetHeight(HeightRGB height)
    {
        this.height = height;
    }

    public void AddHeight(HeightRGB addValue)
    {
        height += addValue;
    }

    public HeightRGB Height
    {
        get
        {
            return height;
        }
    }

    public HeightValues R
    {
        get { return height.R; }
    }

    public TempValues G
    {
        get { return height.G; }
    }

    public WaterValues B
    {
        get { return height.B; }
    }

}
