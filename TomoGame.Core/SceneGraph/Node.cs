using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.SceneGraph;

/// <summary>Base class for all scene graph nodes. Manages parent-child relationships and drives the Initialize/Update/Draw lifecycle. Transform state (position, size, scale, world rect) is defined in Node.Transform.cs.</summary>
public partial class Node
{
    /// <summary>The parent node, or null if this is the root.</summary>
    public Node? Parent { get; private set; }

    private readonly List<Node> _children = [];

    /// <summary>The children of this node.</summary>
    public IReadOnlyCollection<Node> Children => _children;

    private readonly List<Node> _pendingRemovals = [];

    private bool _initialized;
    private bool _destroyed;

    /// <summary>When false, this node and its subtree are skipped during Update.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>When false, this node and its subtree are skipped during Draw.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>True once <see cref="Destroy"/> has been called on this node.</summary>
    public bool IsDestroyed => _destroyed;

    public Node(Node? parent = null)
    {
        parent?.AddChild(this);
    }

    internal void Initialize()
    {
        if (_initialized) return;

        _initialized = true;
        OnInitialize();
        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].Initialize();
        }
    }

    internal void Update(GameTime gameTime)
    {
        if (!Enabled) return;

        OnUpdate(gameTime);
        for (int i = 0; i < _children.Count; i++)
        {
            Node child = _children[i];
            if (!child._destroyed)
                child.Update(gameTime);
        }

        ProcessPendingRemovals();
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        if (!Visible) return;

        OnDraw(spriteBatch);
        for (int i = 0; i < _children.Count; i++)
        {
            Node child = _children[i];
            if (!child._destroyed)
                child.Draw(spriteBatch);
        }

        ProcessPendingRemovals();
    }

    /// <summary>Called once when the node is initialized. Override to set up node state.</summary>
    protected virtual void OnInitialize() { }

    /// <summary>Called every game tick. Override to implement update logic.</summary>
    protected virtual void OnUpdate(GameTime gameTime) { }

    /// <summary>Called every frame during the draw pass. Override to implement drawing.</summary>
    protected virtual void OnDraw(SpriteBatch spriteBatch) { }

    /// <summary>Called once when the node is destroyed. Override to release resources or unregister from external systems.</summary>
    protected virtual void OnDestroy() { }

    /// <summary>Adds a node as a child, reparenting it if necessary. Initializes the child if this node is already initialized.</summary>
    public void AddChild(Node node)
    {
        if (!Dbg.Verify(node != this))
            return;

        if (!Dbg.Verify(!node._destroyed))
            return;

        if (!Dbg.Verify(!IsDescendantOf(node)))
            return;

        if (!Dbg.Verify(!_children.Contains(node)))
            return;

        node.Parent?.RemoveChild(node);
        _children.Add(node);
        node.Parent = this;
        if (_initialized && !node._initialized)
            node.Initialize();
    }

    /// <summary>Removes this node (and its subtree) from the scene graph. Safe to call during Update/Draw: the node and its children stop updating/drawing immediately, and are unlinked from their parents once the current traversal pass finishes.</summary>
    public void Destroy()
    {
        if (_destroyed) return;
        _destroyed = true;

        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].Destroy();
        }

        OnDestroy();
        Parent?.ScheduleRemoval(this);
    }

    private bool IsDescendantOf(Node node)
    {
        Node? current = Parent;
        while (current != null)
        {
            if (current == node) return true;
            current = current.Parent;
        }
        return false;
    }

    private void RemoveChild(Node node)
    {
        if (!Dbg.Verify(_children.Contains(node)))
            return;

        _children.Remove(node);
        node.Parent = null;
    }

    private void ScheduleRemoval(Node node)
    {
        _pendingRemovals.Add(node);
    }

    private void ProcessPendingRemovals()
    {
        if (_pendingRemovals.Count == 0) return;

        foreach (Node node in _pendingRemovals)
        {
            RemoveChild(node);
        }
        _pendingRemovals.Clear();
    }
}