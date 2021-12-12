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
    public static readonly int MAX_HEIGHT = 6;

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

    public void SetSpriteToTile(SpriteRenderer sprite, int x, int y)
    {
        tileGameObject = MonoBehaviour.Instantiate(sprite, new Vector2(x, y), Quaternion.identity);
    }
    public void DrawTile()
    {
        tileGameObject.color = new Color (height.HeightR * 255 / MAX_HEIGHT, height.HeightG * 255 / MAX_HEIGHT, height.HeightB * 255 / MAX_HEIGHT);
    }

}
