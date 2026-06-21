using Godot;
using System.Collections.Generic;

public static class Services
{
    private static readonly Dictionary<System.Type, Node> Cache = new Dictionary<System.Type, Node>();

    public static T Get<T>() where T : Node
    {
        System.Type type = typeof(T);
        if (Cache.TryGetValue(type, out Node cached))
        {
            if (GodotObject.IsInstanceValid(cached))
            {
                return cached as T;
            }
            Cache.Remove(type);
        }

        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            Logger.Error($"Services: No active SceneTree available while resolving {type.Name}.");
            return null;
        }

        string path = $"/root/{type.Name}";
        T service = tree.Root.GetNodeOrNull<T>(path);
        if (service == null)
        {
            Logger.Error($"Services: Autoload '{path}' of type {type.Name} not found.");
            return null;
        }

        Cache[type] = service;
        return service;
    }

    public static void Clear()
    {
        Cache.Clear();
    }
}
