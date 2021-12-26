using UnityEngine;
using System;
using UnityEngine.UI;

public enum BuildingSlot 
{
    E, S, M, L, XL, XXL
}

[Serializable]
public class TileInfo : MonoBehaviour
{
    [SerializeField] private Tile tileSetInMap;
    private SpriteRenderer tileSpriteRenderer;
    private SpriteRenderer LandscapeModificator;
    private SpriteRenderer Resources;
    private SpriteRenderer Building;

    private bool isCopy;

    public bool GetTileSprite()
    {
        if (tileSetInMap.tileSprite != null)
            return true;
        return false;
    }
    public void SetTileInfo(Tile tile, bool _isCopy)
    {
        tileSetInMap = tile;
        tileSpriteRenderer = GetComponent<SpriteRenderer>();
        isCopy = _isCopy;
    }

    public void PrintData()
    {
        Debug.Log($"R = {tileSetInMap.R} / G = {tileSetInMap.G} / B = {tileSetInMap.B}");
    }

    public void SetSpriteToTile()
    {
        tileSpriteRenderer.sprite = tileSetInMap.tileSprite;   
    }

    public void DrawTile(bool r, bool g, bool b)
    {
        int maxHeight = (int)HeightRGB.MAX_HEIGHT;
        float R, G, B;
        R = G = B = 0;
        if (r) R = (float)tileSetInMap.R / (float)maxHeight;
        if (g) G = (float)tileSetInMap.G / (float)maxHeight;
        if (b) B = (float)tileSetInMap.B / (float)maxHeight;
        tileSpriteRenderer.color = new Color(R, G, B);
    }
}


