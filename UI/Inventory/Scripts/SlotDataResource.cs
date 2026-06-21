using Godot;

[GlobalClass]
public partial class SlotDataResource : Resource
{
    [Export]
    public ItemDataResource _ItemData;
    [Export]
    public int _Quantity = 0;

    public Character User { get; set; }
}
