using UnityEngine;

public enum BuildingSlot 
{
    E, S, M, L, XL, XXL
}

public class TileInfo : MonoBehaviour
{
    [SerializeField] private Tile tileSetInMap;
    [SerializeField] private Sprite tileSprite;
    private SpriteRenderer tileSpriteRenderer;

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
        tileSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PrintData()
    {
        Debug.Log($"R = {tileSetInMap.R} / G = {tileSetInMap.G} / B = {tileSetInMap.B}");
    }

    public void SetSpriteToTile(Sprite sprite)
    {
        tileSpriteRenderer.sprite = sprite;    
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


