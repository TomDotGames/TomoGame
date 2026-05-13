using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

[LayoutNode("Transform")]
public partial class Node
{
    private Vector2 _sizeUnscaled;
    private Vector2 _localScale = Vector2.One;
    private Vector2 _localPosition;

    /// <summary>This node's accumulated world-space scale.</summary>
    public Vector2 WorldScale => _worldScale;
    private Vector2 _worldScale = Vector2.One;

    /// <summary>This node's bounding rectangle in world space.</summary>
    public Rect WorldRect => _worldRect;
    private Rect _worldRect;

    /// <summary>This node's position in world space.</summary>
    public Vector2 WorldPosition => _worldRect.Min;

    public Node(Vector2 localPosition, Vector2 size, Node? parent = null) : this(parent)
    {
        _localPosition = localPosition;
        _sizeUnscaled = size;
        ComputeWorldTransform();
    }

    internal Node(XElement element, Node? parent = null) : this(Vector2.Zero, Vector2.Zero, parent)
    {
    }

    public void SetSize(float width, float height)
    {
        SetSize(new Vector2(width, height));
    }

    /// <summary>Sets the size of this node.</summary>
    public void SetSize(Vector2 size)
    {
        _sizeUnscaled = size;
        ComputeWorldTransform();
    }

    /// <summary>Moves this node by the given offset in local space.</summary>
    public void Translate(Vector2 offset)
    {
        _localPosition += offset;
        ComputeWorldTransform();
    }

    private void ComputeWorldTransform()
    {
        Node? parent = Parent;
        Vector2 parentWorldScale = parent?.WorldScale ?? Vector2.One;
        _worldScale = parentWorldScale * _localScale;
        _worldRect.Size = _sizeUnscaled * _worldScale;

        Vector2 parentWorldPosition = parent?.WorldRect.Min ?? Vector2.Zero;
        _worldRect.Min = parentWorldPosition + (_localPosition * parentWorldScale);
    }
}
