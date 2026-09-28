using System.Collections;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using TomoGame.Core.Coroutines;
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

    /// <summary>The node at the top of this node's tree, which for a node in a live scene is its
    /// <see cref="SceneRootNode"/>.</summary>
    public Node Root
    {
        get
        {
            Node root = this;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            return root;
        }
    }

    private bool _initialized;

    /// <summary>Where this node draws relative to the rest of the graph. Higher draws later, so on top. It
    /// accumulates down the tree, so raising a node's order lifts its whole subtree, and nodes sharing an
    /// order keep the graph's own order: parents before children, siblings as they were added.</summary>
    public float ZOrder { get; set; }

    /// <summary>This node's draw order with its ancestors' accumulated, which is the order it actually draws
    /// in. <see cref="CollectDrawList"/> arrives at the same number by walking down.</summary>
    public float WorldZOrder
    {
        get
        {
            float zOrder = 0f;
            for (Node? node = this; node != null; node = node.Parent)
            {
                zOrder += node.ZOrder;
            }
            return zOrder;
        }
    }

    public Node(Node? parent = null)
    {
        parent?.AddChild(this);
    }

    internal void Initialize()
    {
        // a scene can be shown more than once, and re-running OnInitialize would allocate a second set of
        // everything the first run made. AddChild already relies on this flag covering the whole subtree.
        if (_initialized)
            return;

        _initialized = true;
        OnInitialize();

        // by index, for the reason given on Update
        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].Initialize();
        }
    }

    internal void Update(GameTime gameTime)
    {
        OnUpdate(gameTime);

        // by index, not foreach: OnUpdate can add to the tree - DebugDraw parents its overlay onto the scene
        // root on first use - and appending mid-foreach throws. Anything appended is picked up by this walk.
        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].Update(gameTime);
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
    /// <param name="keepWorldPosition">Holds the node still on screen across the reparent, rather than
    /// carrying its local position into the new parent's frame.</param>
    public void AddChild(Node node, bool keepWorldPosition = false)
    {
        if (!Dbg.Verify(node != this))
            return;

        if (!Dbg.Verify(!IsDescendantOf(node)))
            return;

        if (!Dbg.Verify(!_children.Contains(node)))
            return;

        // read before reparenting, since WorldPosition is measured through the old parent; only when actually
        // needed, since reading it this early caches a transform through the old (possibly null) parent that
        // must then be invalidated below rather than trusted
        Vector2 worldPosition = keepWorldPosition ? node.WorldPosition : Vector2.Zero;

        node.Parent?.RemoveChild(node);
        _children.Add(node);
        node.Parent = this;
        node.MarkWorldTransformDirty();

        if (keepWorldPosition)
            node.WorldPosition = worldPosition;

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

    /// <summary>Detaches a child, leaving it parentless. <see cref="AddChild"/> already does this when
    /// reparenting; call it directly only to take a node out of the tree entirely.</summary>
    public void RemoveChild(Node node)
    {
        if (!Dbg.Verify(_children.Contains(node)))
            return;

        _children.Remove(node);
        node.Parent = null;
    }

    // Coroutines
    private readonly List<Coroutine> _coroutines = new();

    /// <summary>Runs a coroutine, keeping hold of it so the node can cut it short later.</summary>
    public void RunCoroutine(Coroutine coroutine)
    {
        // the original never dropped finished tasks, so the list grew for the life of the node
        _coroutines.RemoveAll(running => running.IsFinished);

        if (CoroutineManager.Run(coroutine) != null)
        {
            _coroutines.Add(coroutine);
        }
    }

    public void RunCoroutine(IEnumerator enumerator)
    {
        RunCoroutine(new WrappedEnumerator(enumerator));
    }

    /// <summary>Jumps this node's coroutines to their end state, so it settles without waiting.</summary>
    public void FinishAllCoroutinesImmediately()
    {
        foreach (Coroutine coroutine in _coroutines.ToList())
        {
            coroutine.FinishImmediately();
        }
        _coroutines.Clear();
    }
}