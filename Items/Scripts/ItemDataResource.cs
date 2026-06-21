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

    public bool Use()
    {
        if (_ItemEffects.Length == 0)
        {
            return false;
        }

        foreach (ItemEffectResource effect in _ItemEffects)
        {
            effect.Use();
        }
        return true;

    }



}
