using Microsoft.Xna.Framework;

namespace TomoGame.Core;

/// <summary>A line segment defined by a start and end point.</summary>
public struct Line
{
    /// <summary>The start point of the line segment.</summary>
    public Vector2 Start { get; set; }

    /// <summary>The end point of the line segment.</summary>
    public Vector2 End { get; set; }

    /// <summary>Creates a line segment from <paramref name="start"/> to <paramref name="end"/>.</summary>
    public Line(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Vector from Start to End.</summary>
    public Vector2 Delta => End - Start;
    /// <summary>Distance between Start and End.</summary>
    public float Length => Delta.Length();
    /// <summary>Angle of the line in radians, measured from the positive X axis.</summary>
    public float Angle => MathF.Atan2(Delta.Y, Delta.X);
}