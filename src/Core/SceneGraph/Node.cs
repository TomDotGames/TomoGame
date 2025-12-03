using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace TomoGame.Core.SceneGraph
{
    public class Node
    {
        private Node _parent;
        private readonly List<Node> _children = [];
        
        protected Rect LocalRect => _localRect;
        private readonly Rect _localRect = new();
        
        private readonly Rect _worldRect = new();

        protected Node()
        {
        }
        
        public Node(Node parent)
        {
            Debug.Assert(parent != null);
            parent.AddChild(this);
            _parent = parent;
        }

        ~Node()
        {
            _parent?.RemoveChild(this);
        }
        
        public virtual void Initialize()
        {
            foreach (var child in _children)
            {
                child.Initialize();
            }
        }

        public virtual void Update(double deltaTime)
        {
            foreach (var child in _children)
            {
                child.Update(deltaTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in _children)
            {
                child.Draw(spriteBatch);
            }
        }

        private void AddChild(Node node)
        {
            node._parent?.RemoveChild(node);

            Debug.Assert(!_children.Contains(node));
            _children.Add(node);
            node._parent = this;
        }
        
        private void RemoveChild(Node node)
        {
            Debug.Assert(_children.Contains(node));
            _children.Remove(node);
            node._parent = null;
        }

        public void SetLocalSize(float width, float height)
        {
            _localRect.Size = new Vector2(width, height);
        }
    }
}