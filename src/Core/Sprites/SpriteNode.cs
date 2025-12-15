using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites;

public class SpriteNode : Node
{
    private readonly SpriteRenderer _renderer;
        
    public SpriteNode(Node parent, string spriteSheetPath) : base(parent)
    {
        _renderer = new(spriteSheetPath);
        SetLocalSize(_renderer.Size.X, _renderer.Size.Y);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _renderer.Draw(spriteBatch, LocalRect);
        base.Draw(spriteBatch);
    }
}