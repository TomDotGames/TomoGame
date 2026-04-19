using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.SceneGraph;

/// <summary>Base class for all scene graph nodes. Manages parent-child relationships and drives the Initialize/Update/Draw lifecycle.</summary>
public class Node
{
    /// <summary>The parent node, or null if this is the root.</summary>
    public Node? Parent { get; private set; }

    private readonly List<Node> _children = [];

    /// <summary>The children of this node.</summary>
    public IReadOnlyCollection<Node> Children => _children;

    private bool _initialized;

    public Node(Node? parent = null)
    {
        parent?.AddChild(this);
    }

    internal void Initialize()
    {
        _initialized = true;
        OnInitialize();
        foreach (Node child in _children)
        {
            child.Initialize();
        }
    }

    internal void Update(double deltaTime)
    {
        OnUpdate(deltaTime);
        foreach (Node child in _children)
        {
            child.Update(deltaTime);
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        OnDraw(spriteBatch);
        foreach (Node child in _children)
        {
            child.Draw(spriteBatch);
        }
    }

    /// <summary>Called once when the node is initialized. Override to set up node state.</summary>
    protected virtual void OnInitialize() { }

    /// <summary>Called every game tick. Override to implement update logic.</summary>
    protected virtual void OnUpdate(double deltaTime) { }

    /// <summary>Called every frame during the draw pass. Override to implement drawing.</summary>
    protected virtual void OnDraw(SpriteBatch spriteBatch) { }

    /// <summary>Adds a node as a child, reparenting it if necessary. Initializes the child if this node is already initialized.</summary>
    public void AddChild(Node node)
    {
        if (!Dbg.Verify(node != this))
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
}