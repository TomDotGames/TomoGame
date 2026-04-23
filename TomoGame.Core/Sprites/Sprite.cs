using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.Sprites;

public class Sprite
{
    public Texture2D Texture => _spriteSheet;
    public Rectangle SourceRect => _sourceRect;
    private Texture2D _spriteSheet;
    
    private Rectangle _sourceRect;

    private struct Animation
    {
        public Rectangle FirstFrameRect;
        public int FrameCount;
    }

    private List<Animation> _animations = [];

    public Sprite(Texture2D spriteSheet)
    {
        _spriteSheet = spriteSheet;
        _sourceRect = new Rectangle(0, 0, _spriteSheet.Width, _spriteSheet.Height);
    }
}