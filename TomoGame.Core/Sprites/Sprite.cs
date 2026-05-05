using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites;

/// <summary>A sprite referencing a region of a texture, with optional named animations.</summary>
public class Sprite
{
    private Texture2D _spriteSheet;
    private Rectangle _sourceRect;

    /// <summary>The source texture for this sprite.</summary>
    public Texture2D Texture => _spriteSheet;

    /// <summary>The source rectangle within the texture for this sprite's base frame.</summary>
    public Rectangle SourceRect => _sourceRect;

    /// <summary>Defines a frame-based animation within a sprite sheet.</summary>
    public struct Animation
    {
        /// <summary>The source rectangle of the first frame.</summary>
        public Rectangle FirstFrameRect;

        /// <summary>The number of frames in the animation.</summary>
        public int FrameCount;

        /// <summary>Duration of each frame in seconds. Defaults to 2fps.</summary>
        public float FrameTime = 1f / 2f;

        /// <summary>Creates an animation with the given first frame and frame count.</summary>
        public Animation(Rectangle firstFrameRect, int frameCount)
        {
            FirstFrameRect = firstFrameRect;
            FrameCount = frameCount;
        }
    }

    private Dictionary<string, Animation> _animations = new();

    /// <summary>Creates a sprite from the full extent of a texture.</summary>
    public Sprite(Texture2D spriteSheet)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = new Rectangle(0, 0, _spriteSheet.Width, _spriteSheet.Height);
    }

    /// <summary>Creates a sprite with a specific source rectangle and animation set.</summary>
    public Sprite(Texture2D spriteSheet, Rectangle sourceRect, Dictionary<string, Animation> animations)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = sourceRect;
        _animations = animations;
    }

    /// <summary>Returns the named animation, or null if it does not exist.</summary>
    public Animation? GetAnimation(string name)
    {
        return _animations.TryGetValue(name, out Animation animation) ? animation : null;
    }
}
