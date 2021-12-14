using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum GameResources
{
    none,
    type1,
    type2,
    type3
}
public class TileInfo : Tiles
{
    private Buildings BuildedGameObject;
    private bool canBuild;

    private TradeGoods good; 
    [Range(0, 1)]
    private float restOfResourses;
}


