using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites;

public class SpriteRenderer
{
    private readonly Texture2D _texture;
    
    public SpriteRenderer(string spriteSheetPath)
    {
        _texture = TomoGame.Instance.ResourceManager.GetResource<Texture2D>(spriteSheetPath); // hmmmm....
        Debug.Assert(_texture != null);
    }
    
    public Vector2 Size => new Vector2(_texture.Width, _texture.Height);

    public void Draw(SpriteBatch spriteBatch, Rect worldRect)
    {
        spriteBatch.Draw(_texture, worldRect.ToRectangle(), Color.White);
    }
}