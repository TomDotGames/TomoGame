using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace TDG.SceneGraph
{
    public class SceneNode
    {
        protected Scene OwnerScene => m_scene;
        private Scene m_scene;

        public Vector2 PositionWorld
        {
            get => m_vPositionWorld;
            set
            {
                if (m_vPositionWorld != value)
                {
                    m_vPositionWorld = value;
                    Vector2 vParentPosition = m_parent == null ? Vector2.Zero : m_parent.PositionWorld;
                    m_vPositionLocal = vParentPosition - m_vPositionWorld;
                    OnTransformChanged();
                }
            }
        }
        private Vector2 m_vPositionWorld;

        public Vector2 PositionLocal
        {
            get => m_vPositionLocal;
            set
            {
                if (m_vPositionLocal != value)
                {
                    m_vPositionLocal = value;
                    Vector2 vParentPosition = m_parent == null ? Vector2.Zero : m_parent.PositionWorld;
                    m_vPositionWorld = vParentPosition + m_vPositionLocal;
                    OnTransformChanged();
                }
            }
        }
        private Vector2 m_vPositionLocal;

        public SceneNode Parent => m_parent;
        private SceneNode m_parent;
        protected List<SceneNode> m_children = new List<SceneNode>();

        public SceneNode(SceneNode parent, Scene scene)
        {
            parent?.AddChild(this);
            m_scene = scene;
            m_parent = parent;

            Debug.Assert(m_scene != null);
        }

        public SceneNode(SceneNode parent)
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

        public virtual void Draw(float flDeltaTime, SpriteBatch spriteBatch)
        {
            Debug.Assert(spriteBatch != null);

            foreach (var child in m_children)
            {
                child.Draw(flDeltaTime, spriteBatch);
            }
        }

        public virtual void Update(float flDeltaTime)
        {
            foreach (var child in m_children)
            {
                child.Update(flDeltaTime);
            }
        }

        public void AddChild(SceneNode child)
        {
            Debug.Assert(child != null);
            if (!m_children.Contains(child))
            {
                child.SetParent(this);
                m_children.Add(child);
            }
        }

        public void RemoveChild(SceneNode child)
        {
            Debug.Assert(child != null);
            if (m_children.Contains(child))
            {
                child.SetParent(null);
                m_children.Remove(child);
            }
        }

        protected void SetParent(SceneNode parent)
        {
            if (m_parent != null)
            {
                Debug.Assert(m_parent.m_children.Contains(this));
                m_parent.m_children.Remove(this);
            }

            m_vPositionLocal = parent == null ? Vector2.Zero : PositionWorld - parent.PositionWorld;
            m_parent = parent;
        }

        protected virtual void OnTransformChanged()
        {
            foreach (var child in m_children)
            {
                child.m_vPositionWorld = PositionWorld + child.PositionLocal;
                child.OnTransformChanged();
            }
        }
    }
}
