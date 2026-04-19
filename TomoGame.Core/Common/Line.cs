using Microsoft.Xna.Framework;

namespace TomoGame.Core;

/// <summary>A line segment defined by a start and end point.</summary>
public struct Line
{
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }

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