using Microsoft.Xna.Framework;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Input;

/// <summary>A pointable whose hit area is a node's world rect. The rect is read live rather than copied, so
/// it never needs syncing and cannot go stale when the node moves, resizes or is destroyed.</summary>
public class NodePointable : Pointable
{
    private readonly Node _node;

    /// <summary>When false this pointable ignores pointers even while its node is active.</summary>
    public bool Enabled { get; set; } = true;

    public NodePointable(Node node)
    {
        _node = node;
    }

    public override bool IsPointInside(Vector2 point)
    {
        return Enabled && IsNodeActive() && _node.WorldRect.Contains(point);
    }

    /// <summary>A node only takes input while it and every ancestor is alive, updating and drawing - otherwise
    /// something hidden behind a disabled parent would still be swallowing clicks.</summary>
    private bool IsNodeActive()
    {
        for (Node? node = _node; node != null; node = node.Parent)
        {
            if (node.IsDestroyed || !node.Enabled || !node.Visible)
                return false;
        }

        return true;
    }
}
