using System.Collections.Generic;
using Godot;

public partial class World : Node2D
{
    public TileMapLayer[] _TileMapLayers;

    public override void _EnterTree()
    {
        _TileMapLayers = FindAllMapLayers();
    }

    private TileMapLayer[] FindAllMapLayers()
    {
        List<TileMapLayer> tileMapLayerList = new List<TileMapLayer>();
        foreach (Node child in GetChildren())
        {
            if (child is TileMapLayer tileMapLayer)
            {
                tileMapLayerList.Add(tileMapLayer);
            }
        }
        return tileMapLayerList.ToArray();
    }

    public bool IsPointWalkable(Vector2I position)
    {
        if (_TileMapLayers == null || _TileMapLayers.Length == 0)
        {
            return false;
        }

        var mapPosition = _TileMapLayers[0].LocalToMap(position);
        var tileData = _TileMapLayers[0].GetCellTileData(mapPosition);
        if (tileData == null)
        {
            return true;
        }

        var customData = tileData.GetCustomData("Walkable");
        if (customData.VariantType == Variant.Type.Nil)
        {
            return true;
        }
        return customData.AsBool();
    }
}
