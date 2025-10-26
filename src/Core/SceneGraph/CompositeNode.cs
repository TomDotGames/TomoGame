using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace TomoGame.Core.SceneGraph
{
    public class CompositeNode : Node
    {
        private List<Component> m_components = new List<Component>();
        private List<IUpdatable> m_updatables = new List<IUpdatable>();
        private List<IRenderable> m_renderables = new List<IRenderable>();

        public CompositeNode(Scene scene) : base(scene)
        {
        }

        public CompositeNode(Node parent) : base(parent)
        {
        }

        public void AddComponent(Component component)
        {
            Debug.Assert(!m_components.Contains(component));
            m_components.Add(component);

            IUpdatable updatable = component as IUpdatable;
            if (updatable != null)
            {
                m_updatables.Add(updatable);
            }

            IRenderable renderable = component as IRenderable;
            if (renderable != null)
            {
                m_renderables.Add(renderable);
            }
        }

        public override void Render(float flDeltaTime, SpriteBatch spriteBatch)
        {
            Debug.Assert(spriteBatch != null);

            foreach (IRenderable renderable in m_renderables)
            {
                renderable.Render(flDeltaTime, spriteBatch);
            }
        }

        public override void Update(float flDeltaTime)
        {
            foreach (IUpdatable updatable in m_updatables)
            {
                updatable.Update(flDeltaTime);
            }
        }
    }
}
