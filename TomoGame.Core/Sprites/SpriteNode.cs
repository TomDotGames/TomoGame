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
    private Sprite? _sprite;
    private Rectangle _sourceRect;
    private AnimationPlayer _animationPlayer = new();

    /// <summary>When true, the sprite is rendered flipped horizontally.</summary>
    public bool FlipX { get; set; }

    public float Alpha { get; set; } = 1f; 

    /// <summary>The sprite being drawn. Setting it resizes the node to suit, as loading one by name does.</summary>
    public Sprite? Sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
            if (_sprite == null)
                return;

            _sourceRect = _sprite.SourceRect;
            IntrinsicSize = new Vector2(_sprite.SourceRect.Width, _sprite.SourceRect.Height);
        }
    }

    public SpriteNode(Node? parent = null) : base(parent)
    {
    }
    
    /// <summary>Creates a sprite node using the named sprite from the <see cref="ResourceManager"/>, placed at
    /// the sprite's declared <see cref="Sprites.Sprite.Offset"/>.</summary>
    public SpriteNode(string spriteName, Node? parent = null) : base(parent)
    {
        LoadSprite(spriteName);
        LocalPosition = _sprite?.Offset ?? Vector2.Zero;
    }

    /// <summary>Creates a sprite node using the named sprite from the <see cref="ResourceManager"/>, at an
    /// explicit position rather than the sprite's own offset.</summary>
    public SpriteNode(string spriteName, Vector2 localPosition, Node? parent = null) : base(parent)
    {
        LoadSprite(spriteName);
        LocalPosition = localPosition;
    }

    public override void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? src = element.Attribute("src");
        if (src != null)
        {
            LoadSprite(src.Value);
        }

        base.ApplyLayoutAttributes(element);

        // after the base, so a sprite's own size is in place before anything anchors against it
        XAttribute? anim = element.Attribute("anim");
        if (anim != null)
        {
            XAttribute? mode = element.Attribute("animmode");
            PlayAnimation(anim.Value, ParseAnimationMode(mode?.Value));
        }
    }

    public void LoadSprite(string spriteName)
    {
        Sprite = ResourceManager.Instance!.GetSprite(spriteName);
    }

    /// <summary>Parses a layout's animmode, defaulting to looping.</summary>
    private static AnimationPlayer.AnimationMode ParseAnimationMode(string? mode)
    {
        return mode?.ToLowerInvariant() switch
        {
            "pingpong" => AnimationPlayer.AnimationMode.PingPong,
            "oneshot" => AnimationPlayer.AnimationMode.OneShot,
            _ => AnimationPlayer.AnimationMode.Loop
        };
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);

        // having no sprite is legitimate: the parameterless ctor and a <Sprite> with no src both leave it unset
        if (_sprite == null)
            return;

        SpriteEffects effects = FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        // stretch the source onto the node's rect, so sizing or scaling the node sizes what is drawn, and
        // pivot on the node's own origin so it turns in place. A node left at its sprite's own size and
        // unrotated renders exactly where WorldRect.Min says.
        Vector2 sourceSize = new(_sourceRect.Width, _sourceRect.Height);
        Vector2 renderScale = WorldSize / sourceSize;
        Vector2 origin = OriginUV * sourceSize;

        Color drawColor = Color.White * Alpha;

        spriteBatch.Draw(_sprite.Texture, WorldPosition, _sourceRect, drawColor, WorldRotation, origin,
            renderScale, effects, 0f);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
        if (_sprite == null)
            return;

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
        if (!Dbg.Verify(_sprite, $"cannot play animation '{animationName}': this sprite node has no sprite"))
            return;

        Sprite.Animation? animation = _sprite.GetAnimation(animationName);
        if (Dbg.Verify(animation != null))
        {
            _animationPlayer.PlayAnimation(animation!.Value, mode);
        }
    }
}
