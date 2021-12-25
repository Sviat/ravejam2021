using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile
{
    [SerializeField] private HeightRGB height;
    [SerializeField] private int x, y;

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
    public void SetXY(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public Tile()
    {
    }
    public Tile(HeightRGB _h)
    {
        height = _h;
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
    //public HeightRGB GetHeight() => new HeightRGB(height);

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
        set { height.R = value; }
    }

    public TempValues G
    {
        get { return height.G; }
        set { height.G = value; }
    }

    public WaterValues B
    {
        get { return height.B; }
        set { height.B = value; }
    }

}
