using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public enum BuildingSlot 
{
    E, S, M, L, XL, XXL
}

[Serializable]
public class TileInfo : MonoBehaviour
{
    [SerializeField] public Tile tileSetInMap;
    private SpriteRenderer tileSpriteRenderer;
    [SerializeField] public Transform LandscapeModificator;
    [SerializeField] public Transform Resources;
    [SerializeField] public Transform Building;

    public Transform[] coastPositions;

    private bool isCopy;

    private SpriteRenderer srCoast;
    public bool GetTileSprite()
    {
        if (tileSpriteRenderer == null)
            tileSpriteRenderer = GetComponent<SpriteRenderer>();
        if (tileSetInMap.tileSprite != null)
            return true;
        return false;
    }

    public void SetTileInfo(Tile tile, bool _isCopy)
    {
        tileSetInMap = tile;
        isCopy = _isCopy;
    }

    public void PrintData()
    {
        Debug.Log($"R = {tileSetInMap.R} / G = {tileSetInMap.G} / B = {tileSetInMap.B}");
    }

    public void SetSpriteToTile()
    {
        tileSpriteRenderer.sprite = tileSetInMap.tileSprite;
        LandscapeModificator.GetComponent<SpriteRenderer>().sprite = tileSetInMap.landscapeModificator;   
    }

    public void DrawTile(bool r, bool g, bool b)
    {
        int maxHeight = 8;
        float R, G, B;
        R = G = B = 0;
        if (r) R = (float)tileSetInMap.R / (float)maxHeight;
        if (g) G = (float)tileSetInMap.G / (float)maxHeight;
        if (b) B = (float)tileSetInMap.B / (float)maxHeight;
        tileSpriteRenderer.color = new Color(R, G, B);
    }

    public void SetCoast(int position, Sprite sprite)
    {
        srCoast = coastPositions[position].gameObject.AddComponent<SpriteRenderer>();
        srCoast.sortingOrder = 10;
        srCoast.sprite = sprite;
    }
}


