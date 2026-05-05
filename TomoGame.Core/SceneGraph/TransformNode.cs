using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

/// <summary>A node with a position and size, maintaining both local and world-space rectangles.</summary>
[LayoutNode("Transform")]
public class TransformNode : Node
{
    private Rect _localRect;
    private Rect _worldRect;

    /// <summary>This node's bounding rectangle in local space.</summary>
    public Rect LocalRect => _localRect;

    /// <summary>This node's bounding rectangle in world space.</summary>
    public Rect WorldRect => _worldRect;

    /// <summary>This node's position in local space.</summary>
    public Vector2 LocalPosition => _localRect.Min;

    /// <summary>This node's position in world space.</summary>
    public Vector2 WorldPosition => _worldRect.Min;

    public TransformNode(Vector2 localPosition, Vector2 size, Node? parent = null) :
        base(parent)
    {
        _localRect = new Rect(localPosition, size);
        ComputeWorldRect();
    }

    internal TransformNode(XElement element, Node? parent = null) : this(Vector2.Zero, Vector2.Zero, parent)
    {
    }

    public void SetSize(float width, float height)
    {
        SetSize(new Vector2(width, height));
    }
    
    /// <summary>Sets the size of this node.</summary>
    public void SetSize(Vector2 size)
    {
        _localRect.Size = size;
        ComputeWorldRect();
    }

    /// <summary>Moves this node by the given offset in local space.</summary>
    public void Translate(Vector2 offset)
    {
        _localRect.Min += offset;
        ComputeWorldRect();
    }

    private void ComputeWorldRect()
    {
        Vector2 parentWorldPosition = (Parent as TransformNode)?.WorldPosition ?? Vector2.Zero;
        _worldRect.Min = parentWorldPosition + LocalPosition;
        _worldRect.Size = _localRect.Size; // todo: scaling
    }
}
