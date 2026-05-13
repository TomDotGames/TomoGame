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
    
    public virtual void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? pos = element.Attribute("pos");
        if (pos != null)
        {
            string[] tokens = pos.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!Dbg.Verify(tokens.Length == 4))
                return;
             
            Vector2 selfAnchorUV = AnchorPositionFromString(tokens[0]);
            Vector2 selfAnchor = UVToLocalPosition(selfAnchorUV);
            
            Vector2 parentAnchorUV = AnchorPositionFromString(tokens[1]);
            Vector2 parentAnchor = UVToLocalPosition(parentAnchorUV);
            
            Vector2 offset = new Vector2(float.Parse(tokens[2]), float.Parse(tokens[3]));
            _localPosition = (selfAnchor - parentAnchor) + offset;
            ComputeWorldTransform();
        }
    }

    private Vector2 AnchorPositionFromString(string anchorPos)
    {
        if (!Dbg.Verify(anchorPos.Length == 2))
            return Vector2.Zero;


        Dictionary<char, float> anchorMapY = new()
        {
            { 't', 0.0f },
            { 'c', 0.5f },
            { 'b', 1.0f }
        };
        if (!Dbg.Verify(anchorMapY.TryGetValue(anchorPos[0], out float yAnchor)))
            return Vector2.Zero;
        
        Dictionary<char, float> anchorMapX = new()
        {
            { 'l', 0.0f },
            { 'c', 0.5f },
            { 'r', 1.0f }
        };
        if (!Dbg.Verify(anchorMapX.TryGetValue(anchorPos[1], out float xAnchor)))
            return Vector2.Zero;
        
        return new Vector2(xAnchor, yAnchor);
    }

    private Vector2 UVToLocalPosition(Vector2 uv)
    {
        return uv * _sizeUnscaled * _localScale;
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
        
        foreach (Node child in _children)
        {
            child.ComputeWorldTransform();
        }
    }
}
