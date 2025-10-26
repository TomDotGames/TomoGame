using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace TomoGame.Core.SceneGraph
{
    public class Node
    {
        public Scene Scene => m_scene;
        private Scene m_scene;

        public Rect RectWorld;
        private Rect m_rectWorld;

        public Node Parent => m_parent;
        private Node m_parent;
        protected List<Node> m_children = new List<Node>();

        public Node(Scene scene)
        {
            m_scene = scene;
            m_rectWorld.Size = scene.Size;

            Debug.Assert(m_scene != null);
        }

        public Node(Node parent)
        {
            parent?.AddChild(this);
            m_scene = parent.m_scene;
            m_parent = parent;

            Debug.Assert(m_scene != null);
        }

        public virtual void Initialize()
        {
            foreach (var child in m_children)
            {
                child.Initialize();
            }
        }

        public virtual void Update(float flDeltaTime)
        {
            foreach (var child in m_children)
            {
                child.Update(flDeltaTime);
            }
        }

        public virtual void Render(float flDeltaTime, SpriteBatch spriteBatch)
        {
            foreach (var child in m_children)
            {
                child.Render(flDeltaTime, spriteBatch);
            }
        }

        public void AddChild(Node child)
        {
            Debug.Assert(child != null);
            if (!m_children.Contains(child))
            {
                child.SetParent(this);
                m_children.Add(child);
            }
        }

        public void RemoveChild(Node child)
        {
            Debug.Assert(child != null);
            if (m_children.Contains(child))
            {
                child.SetParent(null);
                m_children.Remove(child);
            }
        }

        protected void SetParent(Node parent)
        {
            if (m_parent != null)
            {
                Debug.Assert(m_parent.m_children.Contains(this));
                m_parent.m_children.Remove(this);
            }

            m_parent = parent;
        }

        protected virtual void OnTransformChanged()
        {
            foreach (var child in m_children)
            {
                child.OnTransformChanged();
            }
        }
    }
}
