using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites;

public class SpriteNode : TransformNode
{
    private readonly Sprite _sprite;

    public SpriteNode(string spriteName, Vector2 localPosition, Node? parent = null) : base(localPosition, Vector2.Zero, parent)
    {
        _sprite = ResourceManager.Instance!.GetSprite(spriteName);
        SetSize(_sprite.SourceRect.Width, _sprite.SourceRect.Height);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);
        
        spriteBatch.Draw(_sprite.Texture, WorldRect.ToRectangle(), _sprite.SourceRect, Color.White);
    }
}