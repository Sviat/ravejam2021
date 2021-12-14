using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TradeGoods
{
    
}

public class TradeGoodsGlobal : TradeGoods
{
    private int globalMoney;
}

public enum Convic { IDLE, EX, LIMIT };

public class TradeGoodsConvinct : TradeGoods
{
            
}

public class TradeGoodsNano : TradeGoods
{

}

public class TradeGoodsMarco : TradeGoods
{

}

public class TradeGoodsMicro : TradeGoods
{

}
