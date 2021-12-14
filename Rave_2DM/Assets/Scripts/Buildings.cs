using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BuildingPlan
{
    TradeGoods tradeGoods;
    BuildingSlot slot;
    HeightRGB landscape; 
}

public abstract class Buildings 
{
    protected Tiles tile; // Where is builded
    protected SpriteRenderer sprite; // sprite for render
    protected BuildingPlan buildingPlan;
}

public class BuildingFactory // For all or for each class?
{
    
}

enum ExploiBuildingTypes
{
    Wood, 
    Metal, 
    Stone, 
    Fish,
    Animals,
    Soil
}
public class ExploitBuildings : Buildings
{
    Dictionary<ExploiBuildingTypes, int> powerUsage;
}

enum FactoryBuildingsTypes
{
    Mountain,
    Jungle,
    Ocean,
    Desert
}
public class FactoryBuildings : Buildings
{
    Dictionary<FactoryBuildingsTypes, int> powerUsage;
}

enum InfrastructureBuildingsTypes
{
    ColonyCenter,
    PowerSolar,
    PowerWind,
    PowerThermal,
    PowerBurn,
    House,
    Tower,
    Cosmodrome
}
public class InfrastructureBuildings : Buildings
{
    Dictionary<InfrastructureBuildingsTypes, int> powerUsage;
}


enum CivilBuildingsTypes
{
    Arena,
    Tavern,
    Hotel,
    Casino,
    Library
}
public class CivilBuildings : Buildings
{
    Dictionary<CivilBuildings, int> powerUsage;
}

enum NativeBuildingsTypes
{
    RuinMountain,
    RuinJungle,
    RuinOcean,
    RuinDesert
}

public class NativeBuildings : Buildings
{
    Dictionary<NativeBuildingsTypes, int> powerUsage;
}