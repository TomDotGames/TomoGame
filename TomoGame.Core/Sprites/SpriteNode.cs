using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites;

/// <summary>A scene node that renders a sprite, with support for animation and horizontal flipping.</summary>
[LayoutNode("Sprite")]
public class SpriteNode : Node
{
    private Sprite _sprite;
    private Rectangle _sourceRect;
    private AnimationPlayer _animationPlayer = new();

    /// <summary>When true, the sprite is rendered flipped horizontally.</summary>
    public bool FlipX { get; set; }

    /// <summary>Creates a sprite node using the named sprite from the <see cref="ResourceManager"/>.</summary>
    public SpriteNode(string spriteName, Vector2 localPosition, Node? parent = null) : base(localPosition, Vector2.Zero, parent)
    {
        LoadSprite(spriteName);
    }

    public override void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? src = element.Attribute("src");
        if (src != null)
        {
            LoadSprite(src.Value);
        }
        
        base.ApplyLayoutAttributes(element);
    }

    private void LoadSprite(string spriteName)
    {
        _sprite = ResourceManager.Instance!.GetSprite(spriteName);
        _sourceRect = _sprite.SourceRect;
        SetSize(_sprite.SourceRect.Width, _sprite.SourceRect.Height);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);
        SpriteEffects effects = FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(_sprite.Texture, WorldPosition, _sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One,
            effects, 0f);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
        _animationPlayer.Update();

        if (_animationPlayer.Animation != null)
        {
            int animOffset = _animationPlayer.CurrentFrame * (_sourceRect.Width + 1);
            _sourceRect.X = _sprite.SourceRect.X + animOffset;
        }
        else
        {
            _sourceRect.X = _sprite.SourceRect.X;
        }
    }

    /// <summary>Starts playing the named animation. Asserts if the animation does not exist on this sprite.</summary>
    public void PlayAnimation(string animationName, AnimationPlayer.AnimationMode mode)
    {
        Sprite.Animation? animation = _sprite.GetAnimation(animationName);
        if (Dbg.Verify(animation != null))
        {
            _animationPlayer.PlayAnimation(animation!.Value, mode);
        }
    }
}
