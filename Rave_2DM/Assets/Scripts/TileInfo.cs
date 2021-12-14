using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingSlot 
{
    E, S, M, L, XL, XXL
}

public class TileInfo : Tiles
{
    private Buildings BuildedGameObject;
    private BuildingSlot buildingSlot;
    private bool canBuild;

    private TradeGoods good; 
    [Range(0, 1)]
    private float restOfResourses;
}


