using Godot;
using System;

[GlobalClass]

public partial class ItemDataResource : Resource
{
    [Export]
    String _Name = "";
    [Export(PropertyHint.MultilineText)]
    public String _Description = "";
    [Export]
    public Texture2D _Texture;

    [ExportCategory("Item Use Effects")]
    [Export]
    ItemEffectResource[] _ItemEffects;

    [ExportCategory("Inventory Properties")]
    [Export]
    public int _MaxStackSize = 99;

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
