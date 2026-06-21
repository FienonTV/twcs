using Godot;

/// <summary>
/// Central registry for hard-coded resource paths.
/// Use these constants instead of repeating "res://..." strings across the codebase.
/// </summary>
public static class ResourcePaths
{
    public const string PlayerScene = "res://Characters/Player/Player.tscn";
    public const string InventoryTres = "res://UI/Inventory/Player_Inventory.tres";
    public const string InventorySlotScene = "res://UI/Inventory/inventory_slot.tscn";
    public const string LogScene = "res://Scenes/Objects/Trees/log.tscn";
    public const string InventoryMenuAutoload = "/root/InventoryMenu";
}
