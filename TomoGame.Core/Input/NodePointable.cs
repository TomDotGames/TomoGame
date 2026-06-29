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
        return _node.WorldRect.Contains(point);
    }
}