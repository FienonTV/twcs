using Godot;

public partial class CollectableComponent : Area2D
{

    [Signal]
    public delegate void OnItemPickedUpEventHandler();
    Item _Item;

    public override void _Ready()
    {
        _Item = GetParent<Item>();
        BodyEntered += OnBodyEntered;


    }


    public void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            Player player = body as Player;
            if (_Item.ItemData != null)
            {
                if (player.InventoryData.AddItem(_Item.ItemData) == true)
                {
                    EmitSignal("OnItemPickedUp");
                }
            }
        }
    }
}


