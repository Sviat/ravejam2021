using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tiles
{
    public HeightRGB height;
    public SpriteRenderer tileGameObject;


    public void SetHeight (int heightR, int heightG, int heightB)
    {
        height.HeightR = heightR;
        height.HeightG = heightG;
        height.HeightB = heightB;
    }
    public void SetHeight(HeightRGB height)
    {
        this.height = height;
    }

    public void SetSpriteToTile(SpriteRenderer sprite, int x, int y, Transform parent)
    {
        tileGameObject = MonoBehaviour.Instantiate(sprite, new Vector2(x, y), Quaternion.identity, parent);
    }
    public void DrawTile()
    {
        int maxHeight = HeightRGB.MAX_HEIGHT;
        float R, G, B;
        R = (float)height.HeightR / (float)maxHeight;
        G = (float)height.HeightG / (float)maxHeight;
        B = (float)height.HeightB / (float)maxHeight;
        tileGameObject.color = new Color (R, G, B);
    }

}
