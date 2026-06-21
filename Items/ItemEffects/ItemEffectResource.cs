using Godot;
using System;

public abstract partial class ItemEffectResource : Resource
{

    [Export]
    String _EffectDescription;

    public virtual void Use()
    {

    }
}