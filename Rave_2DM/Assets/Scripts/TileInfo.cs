using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingSlot 
{
    E, S, M, L, XL, XXL
}

public class TileInfo : MonoBehaviour
{
    private Tiles tileSetInMap;

    private Buildings BuildedGameObject;
    private BuildingSlot buildingSlot;
    private bool canBuild;

    private TradeGoods good; 
    [Range(0, 1)]
    private float restOfResourses;

    public void SetTileInfo(Tiles tile)
    {
        tileSetInMap = tile;
    }

    public void PrintData()
    {
        Debug.Log($"R = {tileSetInMap.height.R} / G = {tileSetInMap.height.G} / B = {tileSetInMap.height.B}");
    }
}


