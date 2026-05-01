using Microsoft.Xna.Framework;

namespace TomoGame.Core;

/// <summary>Provides global access to the current game time.</summary>
public static class Time
{
    private static GameTime _gameTime = new();

    internal static void Set(GameTime gameTime)
    {
        _gameTime = gameTime;
    }

    /// <summary>Total seconds elapsed since the game started.</summary>
    public static float TotalSeconds => (float)_gameTime.TotalGameTime.TotalSeconds;

    /// <summary>Seconds elapsed during the last game tick.</summary>
    public static float TickSeconds => (float)_gameTime.ElapsedGameTime.TotalSeconds;
}
