using Microsoft.Xna.Framework;

namespace TomoGame.Core;

/// <summary>
/// Represents a rectangle defined by floating-point values.
/// </summary>
/// <param name="min">The minimum corner position.</param>
/// <param name="size">The width and height of the rectangle.</param>
public struct Rect(Vector2 min, Vector2 size)
{
    /// <summary>
    /// Gets or sets the minimum corner position of the rectangle.
    /// </summary>
    public Vector2 Min { get; set; } = min;

    /// <summary>
    /// Gets or sets the size of the rectangle.
    /// </summary>
    public Vector2 Size { get; set; } = size;

    /// <summary>
    /// Gets the maximum corner position of the rectangle.
    /// </summary>
    public Vector2 Max => Min + Size;

    /// <summary>Gets the top-left corner of the rectangle.</summary>
    public Vector2 TopLeft => Min;
    /// <summary>Gets the top-right corner of the rectangle.</summary>
    public Vector2 TopRight => new(Max.X, Min.Y);
    /// <summary>Gets the bottom-left corner of the rectangle.</summary>
    public Vector2 BottomLeft => new(Min.X, Max.Y);
    /// <summary>Gets the bottom-right corner of the rectangle.</summary>
    public Vector2 BottomRight => Max;

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public float Width => Size.X;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public float Height => Size.Y;

    /// <summary>
    /// Gets a rectangle with zero position and size.
    /// </summary>
    public static Rect Zero => new(Vector2.Zero, Vector2.Zero);

    /// <summary>
    /// Converts this rectangle to a <see cref="Rectangle"/> with integer coordinates.
    /// </summary>
    /// <returns>A new <see cref="Rectangle"/> with truncated integer coordinates.</returns>
    public readonly Rectangle ToRectangle()
    {
        return new Rectangle((int)Min.X, (int)Min.Y, (int)Size.X, (int)Size.Y);
    }
}
