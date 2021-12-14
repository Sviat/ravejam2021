using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : Tiles
{
    private Buildings BuildedGameObject;
    private bool canBuild;

    private TradeGoods good; 
    [Range(0, 1)]
    private float restOfResourses;
}


