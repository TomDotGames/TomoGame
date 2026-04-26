using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.Sprites;

public class Sprite
{
    public Texture2D Texture => _spriteSheet;
    public Rectangle SourceRect => _sourceRect;
    private Texture2D _spriteSheet;
    
    private Rectangle _sourceRect;

    public struct Animation
    {
        public Rectangle FirstFrameRect;
        public int FrameCount;
    }

    private Dictionary<string, Animation> _animations = new();

    public Sprite(Texture2D spriteSheet)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = new Rectangle(0, 0, _spriteSheet.Width, _spriteSheet.Height);
    }

    public Sprite(Texture2D spriteSheet, Rectangle sourceRect, Dictionary<string, Animation> animations)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = sourceRect;
        _animations = animations;
    }
    
    public Animation? GetAnimation(string name)
    {
        return _animations.TryGetValue(name, out Animation animation) ? animation : null;
    }
}