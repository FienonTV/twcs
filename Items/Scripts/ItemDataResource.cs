using Godot;
using System;

[GlobalClass]

public partial class ItemDataResource : Resource
{
    [Export]
    public string ItemName = "";
    [Export(PropertyHint.MultilineText)]
    public string Description = "";
    [Export]
    public Texture2D Texture;

    [ExportCategory("Item Use Effects")]
    [Export]
    ItemEffectResource[] _ItemEffects;

    [ExportCategory("Inventory Properties")]
    [Export]
    public int MaxStackSize = 99;

    public bool Use(Character user)
    {
        if (_ItemEffects == null || _ItemEffects.Length == 0)
        {
            return false;
        }

        foreach (ItemEffectResource effect in _ItemEffects)
        {
            effect?.Use(user);
        }
        return true;

    }



}
