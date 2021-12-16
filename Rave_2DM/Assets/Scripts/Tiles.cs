using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tiles
{
    public HeightRGB height;
    public SpriteRenderer tileGameObject;


    public void SetHeight (int heightR, int heightG, int heightB)
    {
        height = new HeightRGB(heightR, heightG, heightB);
    }
    public void SetHeight(HeightRGB height)
    {
        this.height = height;
    }

    public void SetSpriteToTile(SpriteRenderer sprite, int x, int y, Transform parent)
    {
        tileGameObject = MonoBehaviour.Instantiate(sprite, new Vector2(x, y), Quaternion.identity, parent);
        tileGameObject.GetComponent<TileInfo>().SetTileInfo(this);
    }
    public void DrawTile()
    {
        int maxHeight = HeightRGB.MAX_HEIGHT;
        float R, G, B;
        R = (float)height.R / (float)maxHeight;
        G = (float)height.G / (float)maxHeight;
        B = (float)height.B / (float)maxHeight;
        tileGameObject.color = new Color (R, G, B);
    }

}
