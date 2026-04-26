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

    private Animation[] _animations;

    public Sprite(Texture2D spriteSheet)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = new Rectangle(0, 0, _spriteSheet.Width, _spriteSheet.Height);
    }

    public Sprite(Texture2D spriteSheet, Rectangle sourceRect, Animation[] animations)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = sourceRect;
        _animations = animations;
    }
}