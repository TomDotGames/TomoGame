using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.Text;

/// <summary>A monospaced font stored as a grid of glyphs in a texture. Characters run left to right, top to
/// bottom, starting at <see cref="FirstChar"/>, so a character's cell is found by index arithmetic rather
/// than a lookup table.</summary>
public class BitmapFont
{
    /// <summary>The sheet holding the glyphs.</summary>
    public Texture2D Texture { get; }

    /// <summary>Width of one glyph cell, in source pixels. Also the advance between characters.</summary>
    public int CellWidth { get; }

    /// <summary>Height of one glyph cell, in source pixels.</summary>
    public int CellHeight { get; }

    /// <summary>The first character in the sheet.</summary>
    public char FirstChar { get; }

    private readonly int _columns;
    private readonly int _glyphCount;

    public BitmapFont(Texture2D texture, int cellWidth, int cellHeight, char firstChar, int columns)
    {
        Dbg.Assert(cellWidth > 0 && cellHeight > 0 && columns > 0);

        Texture = texture;
        CellWidth = cellWidth;
        CellHeight = cellHeight;
        FirstChar = firstChar;
        _columns = columns;

        int rows = texture.Height / cellHeight;
        _glyphCount = rows * columns;
    }

    /// <summary>The source rect of a character's glyph. Characters outside the sheet fall back to a space, so
    /// unexpected text leaves a gap rather than drawing garbage from elsewhere in the sheet.</summary>
    public Rectangle GetGlyphRect(char character)
    {
        int index = character - FirstChar;
        if (index < 0 || index >= _glyphCount)
            index = ' ' - FirstChar;

        return new Rectangle((index % _columns) * CellWidth, (index / _columns) * CellHeight, CellWidth, CellHeight);
    }

    /// <summary>The size of a single line of text, in source pixels.</summary>
    public Vector2 Measure(string text)
    {
        return new Vector2(text.Length * CellWidth, CellHeight);
    }
}
