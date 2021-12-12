using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Resources
{
    none,
    type1,
    type2,
    type3
}

public class Tiles
{
    public HeightRGB height;
    public SpriteRenderer tileGameObject;
    //private GameObject BuildedGameObject;
    //private bool canBuild;

   // private Resources resource; //Перенести в клетки по типу
    //[Range(0, 1)]
    //private float restOfResourses;

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
