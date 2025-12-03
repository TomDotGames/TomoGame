using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites
{
    public class SpriteNode : Node
    {
        private readonly Texture2D _texture;
        
        public SpriteNode(Node parent, Texture2D texture) : base(parent)
        {
            Debug.Assert(texture != null);
            _texture = texture;
            SetLocalSize(texture.Width, texture.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, LocalRect.ToRectangle(), Color.White);
            base.Draw(spriteBatch);
        }
    }
}
