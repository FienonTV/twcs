using System;
using System.Collections.Generic;
using Godot;

public partial class World : Node2D
{
    Rect2I MapRect = new Rect2I();
    public TileMapLayer[] _TileMapLayers;
    public AStarGrid2D _AstarGrid;

    public bool _AstarGridEnabled = false;

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        Player playerInstance = GameManager._PlayerScene.Instantiate() as Player;
        if(playerInstance != null) 
            Console.WriteLine("Player added to world");

        _AstarGrid = new AStarGrid2D();
        _TileMapLayers = FindAllMapLayers();

        int maximumWidth = 0;
        int maximumHeight = 0;
        foreach (TileMapLayer mapLayer in _TileMapLayers)
        {
            int tempWidth = mapLayer.GetUsedRect().End.X;
            int temHeight = mapLayer.GetUsedRect().End.Y;
            if (tempWidth > maximumWidth)
                maximumWidth = tempWidth;
            if (temHeight > maximumHeight)
                maximumHeight = temHeight;
        }

        GD.Print("The current Map size is X: " + maximumWidth + " Y: " + maximumHeight);
        Vector2I TileMapSize = new Vector2I(maximumWidth, maximumHeight);

        MapRect = new Rect2I(Vector2I.Zero, TileMapSize);
        Vector2I TileSize = _TileMapLayers[0].TileSet.TileSize;

        _AstarGrid.Region = MapRect;
        _AstarGrid.CellSize = TileSize;
        _AstarGrid.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
        _AstarGrid.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
        _AstarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        _AstarGrid.Update();

        if (_AstarGrid == null)
        {
            GD.Print("A Star grid not ready successfully");
        }

        foreach (TileMapLayer mapLayer in _TileMapLayers)
        {
            //Get TileMaplLayersSize
            Vector2I LayerSize = mapLayer.GetUsedRect().End;

            for (int i = 0; i < LayerSize.X; i++)
            {
                for (int j = 0; j < LayerSize.Y; j++)
                {
                    Vector2I coords = new Vector2I(i, j);
                    var TileData = mapLayer.GetCellTileData(coords);
                    if (TileData != null && (bool)TileData.GetCustomData("Walkable") == false)
                    {
                        _AstarGrid.SetPointSolid(new Vector2I(i, j));
                    }
                }
            }
        }

        _AstarGridEnabled = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    private TileMapLayer[] FindAllMapLayers()
    {
        List<TileMapLayer> TileMapLayerList = new List<TileMapLayer>();
        foreach (Node child in GetChildren())
        {
            if (child is TileMapLayer tileMapLayer)
            {
                TileMapLayerList.Add(tileMapLayer);
            }
        }
        return TileMapLayerList.ToArray();
    }

    public bool IsPointWalkable(Vector2I position)
    {
        var MapPosition = _TileMapLayers[0].LocalToMap(position);

        if (MapRect.HasPoint(MapPosition) && _AstarGrid.IsPointSolid(MapPosition) == false)
        {
            return true;
        }
        return false;
    }
}
