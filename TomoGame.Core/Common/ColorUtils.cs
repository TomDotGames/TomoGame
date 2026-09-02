using Microsoft.Xna.Framework;

namespace TomoGame.Core;

/// <summary>Colour helpers.</summary>
public static class ColorUtils
{
    /// <summary>Blends <paramref name="over"/> onto <paramref name="under"/> by its alpha.</summary>
    public static Color Blend(Color under, Color over)
    {
        return Color.Lerp(under, over, over.A / 255.0f);
    }

    /// <summary>Returns a random opaque colour.</summary>
    public static Color Random()
    {
        return new Color(
            System.Random.Shared.Next(0, 256),
            System.Random.Shared.Next(0, 256),
            System.Random.Shared.Next(0, 256));
    }
}
