using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TomoGame.Core.SceneGraph
{
    public interface IUpdatable
    {
        void Update(float flDeltaTime);
    }

    public interface IRenderable
    {
        void Render(float flDeltaTime, SpriteBatch spriteBatch);
    }

    public abstract class Component
    {
        public CompositeNode Node => m_node;
        private readonly CompositeNode m_node;

        public Component(CompositeNode node)
        {
            m_node = node;
            m_node.AddComponent(this);
        }
    }
}
