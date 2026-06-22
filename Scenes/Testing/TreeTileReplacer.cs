using Godot;

/// <summary>
/// Replaces tree atlas tiles in a TileMapLayer with real SmallTree/LargeTree scene instances.
/// This makes the trees interactable (hurtbox, health, proper Y-sorting).
/// </summary>
public partial class TreeTileReplacer : Node2D
{
    [Export]
    private TileMapLayer _TreesLayer;

    [Export]
    private PackedScene _SmallTreeScene;

    [Export]
    private PackedScene _LargeTreeScene;

    [Export]
    private float _LargeTreeProbability = 0.3f;

    public override void _Ready()
    {
        if (_TreesLayer == null)
        {
            Logger.Error("TreeTileReplacer: TreesLayer is not assigned.");
            return;
        }

        if (_SmallTreeScene == null)
        {
            _SmallTreeScene = GD.Load<PackedScene>("res://Scenes/Objects/Trees/small_tree.tscn");
        }

        if (_LargeTreeScene == null)
        {
            _LargeTreeScene = GD.Load<PackedScene>("res://Scenes/Objects/Trees/large_tree.tscn");
        }

        if (_SmallTreeScene == null)
        {
            Logger.Error("TreeTileReplacer: SmallTreeScene could not be loaded.");
            return;
        }

        Godot.Collections.Array<Vector2I> usedCells = _TreesLayer.GetUsedCells();
        if (usedCells.Count == 0)
        {
            Logger.Info("TreeTileReplacer: No tree tiles to replace.");
            return;
        }

        Logger.Info($"TreeTileReplacer: Replacing {usedCells.Count} tree tiles with scene instances.");

        foreach (Vector2I cell in usedCells)
        {
            Vector2 worldPosition = _TreesLayer.MapToLocal(cell);
            PackedScene treeScene = SelectTreeScene();
            if (treeScene == null)
            {
                continue;
            }

            Node2D treeInstance = treeScene.Instantiate() as Node2D;
            if (treeInstance == null)
            {
                Logger.Error("TreeTileReplacer: Failed to instantiate tree scene.");
                continue;
            }

            treeInstance.GlobalPosition = _TreesLayer.ToGlobal(worldPosition);
            treeInstance.ZAsRelative = true;
            treeInstance.YSortEnabled = true;
            AddChild(treeInstance);
        }

        _TreesLayer.Clear();
        Logger.Info("TreeTileReplacer: Tree replacement complete.");
    }

    private PackedScene SelectTreeScene()
    {
        if (_LargeTreeScene == null)
        {
            return _SmallTreeScene;
        }

        return GD.Randf() < _LargeTreeProbability ? _LargeTreeScene : _SmallTreeScene;
    }
}
