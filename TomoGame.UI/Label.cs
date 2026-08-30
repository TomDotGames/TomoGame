using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Text;

namespace TomoGame.UI;

/// <summary>Draws a single line of text with a <see cref="BitmapFont"/>. The node sizes itself to its text,
/// so positioning it works the same as for any other node, and the text is drawn to fill the node's rect, so
/// resizing the node scales the text.</summary>
[LayoutNode("Label")]
public class Label : Node
{
    private BitmapFont? _font;
    private string _text = string.Empty;
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

    /// <summary>The height of the text in scene units. Defaults to the font's own cell height, which draws one
    /// source pixel per scene unit, the same as a sprite.</summary>
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
    public BitmapFont? Font
    {
        get => _font;
        set
        {
            _font = value;
            ResizeToText();
        }
    }

    public Label(Node? parent = null) : base(parent)
    {
        _font = ResourceManager.Instance?.DefaultFont;
        _textHeight = _font?.CellHeight ?? 0f;
    }

    public Label(string text, BitmapFont font, float textHeight, Node? parent = null) : base(parent)
    {
        _font = font;
        _text = text;
        _textHeight = textHeight;
        ResizeToText();
    }

    public override void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? font = element.Attribute("font");
        if (font != null)
            _font = ResourceManager.Instance!.GetFont(font.Value);

        XAttribute? height = element.Attribute("height");
        if (height != null)
            _textHeight = float.Parse(height.Value, CultureInfo.InvariantCulture);

        XAttribute? color = element.Attribute("color");
        if (color != null)
            Color = ParseColor(color.Value);

        XAttribute? text = element.Attribute("text");
        if (text != null)
            _text = text.Value;

        // size before the base class positions us, because a position is anchored against our own size
        ResizeToText();

        base.ApplyLayoutAttributes(element);
    }

    /// <summary>Parses "r g b" or "r g b a", each 0-255.</summary>
    private static Color ParseColor(string value)
    {
        string[] tokens = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (!Dbg.Verify(tokens.Length is 3 or 4, "color expects 3 or 4 values, eg: color=\"255 255 255\""))
            return Color.White;

        return new Color(
            int.Parse(tokens[0], CultureInfo.InvariantCulture),
            int.Parse(tokens[1], CultureInfo.InvariantCulture),
            int.Parse(tokens[2], CultureInfo.InvariantCulture),
            tokens.Length == 4 ? int.Parse(tokens[3], CultureInfo.InvariantCulture) : 255);
    }

    /// <summary>Sizes the node to its text at the current height, keeping the font's aspect ratio.</summary>
    private void ResizeToText()
    {
        if (_font == null)
            return;

        Vector2 textPixels = _font.Measure(_text);
        float scale = _textHeight / _font.CellHeight;
        IntrinsicSize = textPixels * scale;
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);

        if (_font == null || _text.Length == 0)
            return;

        Vector2 textPixels = _font.Measure(_text);
        if (textPixels.X <= 0f || textPixels.Y <= 0f)
            return;

        // map the text onto the node's rect, so resizing the node scales the text with it
        Rect worldRect = WorldRect;
        Vector2 renderScale = worldRect.Size / textPixels;

        for (int i = 0; i < _text.Length; i++)
        {
            Rectangle glyphRect = _font.GetGlyphRect(_text[i]);
            Vector2 position = worldRect.Min + new Vector2(i * _font.CellWidth * renderScale.X, 0f);

            spriteBatch.Draw(_font.Texture, position, glyphRect, Color, 0f, Vector2.Zero, renderScale,
                SpriteEffects.None, 0f);
        }
    }
}
