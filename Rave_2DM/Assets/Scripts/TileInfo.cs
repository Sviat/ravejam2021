using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingSlot 
{
    E, S, M, L, XL, XXL
}

public class TileInfo : MonoBehaviour
{
    [SerializeField] private Tile tileSetInMap;
    [SerializeField] private SpriteRenderer LandscapeModificator;
    [SerializeField] private SpriteRenderer Resources;
    [SerializeField] private SpriteRenderer Building;

    [SerializeField] private BuildingSlot buildingSlot;
    private Buildings BuiltGameObject = null;
    
    private bool canBuild;

    private TradeGoodsTypes good = TradeGoodsTypes.Null;
    [Range(0, 1)]
    private float remainingResourses = 1;

    public void SetTileInfo(Tile tile)
    {
        tileSetInMap = tile;
    }

    public void PrintData()
    {
        Debug.Log($"R = {tileSetInMap.height.R} / G = {tileSetInMap.height.G} / B = {tileSetInMap.height.B}");
    }
}


