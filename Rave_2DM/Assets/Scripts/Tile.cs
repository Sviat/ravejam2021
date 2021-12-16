using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] public class Tile
{
    [SerializeField] public HeightRGB height;
    public SpriteRenderer tileGameObject;


    public void SetHeight (int heightR, int heightG, int heightB)
    {
        height = new HeightRGB(heightR, heightG, heightB);
    }
    public void SetHeight(HeightRGB height)
    {
        this.height = height;
    }

    public void SetSpriteToTile(SpriteRenderer sprite, int x, int y, Transform parent, Sprite[] tileSprites)
    {
        tileGameObject = MonoBehaviour.Instantiate(sprite, new Vector2(x, y), Quaternion.identity, parent);
        tileGameObject.GetComponent<TileInfo>().SetTileInfo(this);

        int index = 0;
        switch (height.R)
        {
            case HeightValues.R0_DEEP_OCEAN:
                index = 16;
                break;
            case HeightValues.R2_OCEAN:
                index = 17;
                break;
            case HeightValues.R3_COAST:
                index = 2;
                break;
            case HeightValues.R6_MOUNTAINS:
                index = 4;
                break;

            default:
                break;

        }
        tileGameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[index];

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
