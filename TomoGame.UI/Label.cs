using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Text;

namespace TomoGame.UI;

/// <summary>Draws a single line of text with a <see cref="BitmapFont"/>. The node sizes itself to its text,
/// so anchoring it positions the text the way it would any other node. Resizing it afterwards stretches the
/// text to fit, the same way a <see cref="Core.Sprites.SpriteNode"/> stretches to its rect.</summary>
public class Label : Node
{
    private readonly BitmapFont _font;
    private string _text;
    private float _textHeight;

    /// <summary>The text to draw. Setting it resizes the node.</summary>
    public string Text
    {
        get => _text;
        set
        {
            if (_text == value)
                return;

            _text = value;
            ResizeToText();
        }
    }

    /// <summary>The height of the text in scene units. Setting it resizes the node.</summary>
    public float TextHeight
    {
        get => _textHeight;
        set
        {
            if (Math.Abs(_textHeight - value) < float.Epsilon)
                return;

            _textHeight = value;
            ResizeToText();
        }
    }

    /// <summary>The colour the glyphs are tinted with.</summary>
    public Color Color { get; set; } = Color.White;

    /// <summary>The font the label draws with.</summary>
    public BitmapFont Font => _font;

    public Label(string text, BitmapFont font, float textHeight, Node? parent = null) : base(parent)
    {
        _font = font;
        _text = text;
        _textHeight = textHeight;
        ResizeToText();
    }

    /// <summary>Sizes the node to the text at its current height, keeping the font's aspect ratio.</summary>
    private void ResizeToText()
    {
        Vector2 textPixels = _font.Measure(_text);
        float scale = _textHeight / _font.CellHeight;
        SetIntrinsicSize(textPixels * scale);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);

        Vector2 textPixels = _font.Measure(_text);
        if (textPixels.X <= 0f || textPixels.Y <= 0f)
            return;

        // same mapping a SpriteNode uses: text space onto the node's rect, then the graph's scale on top
        Vector2 renderScale = (IntrinsicSize / textPixels) * WorldScale;

        // the label's anchor expressed in text pixels; each glyph's origin is measured back from it so the
        // whole line rotates about the anchor as one piece
        Vector2 anchorPixels = textPixels * Anchor;

        for (int i = 0; i < _text.Length; i++)
        {
            Rectangle glyphRect = _font.GetGlyphRect(_text[i]);
            Vector2 origin = anchorPixels - new Vector2(i * _font.CellWidth, 0f);

            spriteBatch.Draw(_font.Texture, WorldAnchorPosition, glyphRect, Color, WorldRotation, origin,
                renderScale, SpriteEffects.None, 0f);
        }
    }
}
