using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites
{
    public class SpriteRenderer : Component, IRenderable
    {
        private readonly Texture2D m_texture;
        Vector2 Size => m_texture != null ? m_texture.Bounds.Size.ToVector2() : Vector2.Zero;

        public SpriteRenderer(CompositeNode node, Texture2D texture) : base(node)
        {
            Debug.Assert(texture != null);
            m_texture = texture;
            node.RectWorld.Expand(Size);
        }

        void IRenderable.Render(float flDeltaTime, SpriteBatch spriteBatch)
        {
            Rect rectWindow = Node.Scene.SceneToWindowRect(Node.RectWorld);
            spriteBatch.Draw(m_texture, rectWindow.ToRectangle(), Color.White);
        }
    }
}
