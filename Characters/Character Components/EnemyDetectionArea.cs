using System.Collections.Generic;
using Godot;

public partial class EnemyDetectionArea : Area2D
{
    public List<Player> checkForEnemies()
    {
        List<Player> players = new List<Player>();
        foreach (Character character in GetOverlappingBodies())
        {
            if (character is Player)
            {
                players.Add((Player)character);
            }
        }
        return players;
    }
}
