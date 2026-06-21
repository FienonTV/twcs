using Godot;

[GlobalClass]
public partial class SlotDataResource : Resource
{
    [Export]
    public ItemDataResource ItemData;
    [Export]
    public int Quantity = 0;

    public Character User { get; set; }
}
