using Microsoft.Xna.Framework;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Input;

public class NodePointable : Pointable
{
    private Node _node;
    
    public NodePointable(Node node)
    {
        _node = node;
    }
    
    public override bool IsPointInside(Vector2 point)
    {
        return IsInActiveScene() && _node.WorldRect.Contains(point);
    }

    /// <summary>A scene that is not showing is kept alive, and its pointables stay registered, so without this
    /// a node in a hidden scene still competes for the pointer - and wins it outright if it was registered
    /// first, leaving the visible node under the cursor unable to react.</summary>
    private bool IsInActiveScene()
    {
        Node root = _node;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        return ReferenceEquals(root, GameBase.Instance?.SceneRoot);
    }
}