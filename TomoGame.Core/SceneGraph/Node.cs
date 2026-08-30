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

    private bool _initialized;

    /// <summary>Where this node draws relative to the rest of the graph. Higher draws later, so on top. It
    /// accumulates down the tree, so raising a node's order lifts its whole subtree, and nodes sharing an
    /// order keep the graph's own order: parents before children, siblings as they were added.</summary>
    public float ZOrder { get; set; }

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

    internal void Update(GameTime gameTime)
    {
        OnUpdate(gameTime);
        foreach (Node child in _children)
        {
            child.Update(gameTime);
        }
    }

    /// <summary>Walks the subtree in graph order, appending everything that should draw. Order is decided by
    /// sorting the result rather than by the walk itself, so a node's <see cref="ZOrder"/> can lift it above
    /// nodes that come later in the tree.</summary>
    internal void CollectDrawList(List<DrawEntry> drawList, float inheritedZOrder)
    {
        float zOrder = inheritedZOrder + ZOrder;

        // the index is the tie-break that keeps equal orders in graph order, and makes the sort total so it
        // cannot fall back on the arbitrary ordering an unstable sort gives equal keys
        drawList.Add(new DrawEntry(this, zOrder, drawList.Count));

        foreach (Node child in _children)
        {
            child.CollectDrawList(drawList, zOrder);
        }
    }

    /// <summary>Draws just this node, without its children. The scene root drives this once the draw list is
    /// in order.</summary>
    internal void DrawSelf(SpriteBatch spriteBatch)
    {
        OnDraw(spriteBatch);
    }

    /// <summary>Called once when the node is initialized. Override to set up node state.</summary>
    protected virtual void OnInitialize() { }

    /// <summary>Called every game tick. Override to implement update logic.</summary>
    protected virtual void OnUpdate(GameTime gameTime) { }

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