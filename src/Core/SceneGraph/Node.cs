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

        public Rect RectWorld => m_rectWorld;
        private Rect m_rectWorld = new Rect();

        private Vector2 m_positionLocal;

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

        public void SetPositionLocal(Vector2 vPosition, Vector2 vParentAnchorUV)
        {
            SetPositionLocal(vPosition, vParentAnchorUV, m_rectWorld.OriginUV);
        }

        public void SetPositionLocal(Vector2 vPositionLocal, Vector2 vParentAnchorUV, Vector2 vOriginUV)
        {
            if (m_parent == null)
            {
                SetPositionWorld(vPositionLocal, vOriginUV);
                return;
            }

            Vector2 vParentAnchorWorld = m_parent.RectWorld.UVToWorldCoords(vParentAnchorUV);
            SetPositionWorld(vParentAnchorWorld, vOriginUV);
        }

        public void SetPositionWorld(Vector2 vPositionWorld)
        {
            SetPositionWorld(vPositionWorld, m_rectWorld.OriginUV);
        }

        public void SetPositionWorld(Vector2 vPositionWorld, Vector2 vOriginUV)
        {
            // vOriginUV doesn't change the origin of the rect, but uses it for positioning
            Vector2 vOriginOffsetUV = m_rectWorld.OriginUV - vOriginUV;
            Vector2 vOriginLocal = m_rectWorld.UVToLocalCoords(vOriginOffsetUV);
            m_rectWorld.Position = vPositionWorld - vOriginLocal;

            m_positionLocal = m_parent == null ? m_rectWorld.Position : m_rectWorld.Position - m_parent.m_rectWorld.Position;

            OnTransformUpdated();
        }

        private void OnTransformUpdated()
        {
            if (m_parent != null)
            {
                m_rectWorld.Position = m_parent.m_rectWorld.Position + m_positionLocal;
            }

            foreach (Node child in m_children)
            {
                child.OnTransformUpdated();
            }
        }

        public void Translate(Vector2 vTranslation)
        {
            SetPositionWorld(m_rectWorld.Position + vTranslation);
        }
    }
}
