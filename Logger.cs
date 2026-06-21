using Godot;

/// <summary>
/// Simple project-wide logger with levels.
/// Debug logs are compiled out in release builds if desired, or filtered by level.
/// </summary>
public static class Logger
{
    public enum Level
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static Level CurrentLevel { get; set; } = Level.Debug;

    public static void Debug(string message)
    {
        if (CurrentLevel <= Level.Debug)
        {
            GD.Print("[DBG] " + message);
        }
    }

    public static void Info(string message)
    {
        if (CurrentLevel <= Level.Info)
        {
            GD.Print("[INF] " + message);
        }
    }

    public static void Warning(string message)
    {
        if (CurrentLevel <= Level.Warning)
        {
            GD.PushWarning("[WRN] " + message);
        }
    }

    public static void Error(string message)
    {
        if (CurrentLevel <= Level.Error)
        {
            GD.PushError("[ERR] " + message);
        }
    }
}
