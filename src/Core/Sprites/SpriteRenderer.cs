using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites
{
    public class SpriteRenderer : SceneGraph.Component, SceneGraph.IRenderable
    {
        private readonly Texture2D m_texture;

        public SpriteRenderer(in Texture2D texture)
        {
            Debug.Assert(texture != null);
            m_texture = texture;
        }
    
        void IRenderable.Render(float flDeltaTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture, Vector2.Zero, Color.White);
        }
    }
}
