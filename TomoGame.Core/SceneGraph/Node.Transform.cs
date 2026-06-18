using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

[LayoutNode("Transform")]
public partial class Node
{
    private Rect _localRect; // the rect of this transform with intrinsic size, in parent's intrinsic space

    public Rect WorldRect => _worldRect;
    public Vector2 WorldPosition => _worldRect.Min;
    private Rect _worldRect;
    
    private Vector2 _localScale = Vector2.One;

    public Vector2 WorldScale => _worldScale;
    private Vector2 _worldScale = Vector2.One;

    public Node(Vector2 localPosition, Vector2 size, Node? parent = null) : this(parent)
    {
        _localRect.Min = localPosition;
        _localRect.Size = size;
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
            Vector2 parentAnchorUV = AnchorPositionFromString(tokens[1]);
            Vector2 offset = new Vector2(float.Parse(tokens[2]), float.Parse(tokens[3]));
            
            SetPositionInParentSpace(selfAnchorUV, parentAnchorUV, offset);
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

    public void SetIntrinsicSize(float width, float height)
    {
        SetIntrinsicSize(new Vector2(width, height));
    }

    public void SetIntrinsicSize(Vector2 size)
    {
        _localRect.Size = size;
        ComputeWorldTransform();
    }

    public void SetPositionInParentSpace(Vector2 selfAnchorUV, Vector2 parentAnchorUV, Vector2 offset)
    {
        Vector2 parentAnchor = Parent?.UVToIntrinsicPosition(parentAnchorUV) ?? Vector2.Zero;
        Vector2 selfAnchor = UVToIntrinsicPosition(selfAnchorUV);
        _localRect.Min = (parentAnchor - selfAnchor) + offset;
        
        ComputeWorldTransform();
    }

    private void ComputeWorldTransform()
    {
        Vector2 parentWorldScale = Parent != null ? Parent.WorldScale : Vector2.One;
        Vector2 parentWorldPosition = Parent != null ? Parent.WorldPosition : Vector2.Zero;
        _worldScale = _localScale * parentWorldScale;
        _worldRect.Min = parentWorldPosition + (_localRect.Min * parentWorldScale);
        _worldRect.Size = _localRect.Size * _worldScale;

        foreach (Node child in _children)
        {
            child.ComputeWorldTransform();
        }
    }

    private Vector2 UVToIntrinsicPosition(Vector2 UV)
    {
        return new Vector2(UV.X * _localRect.Width, UV.Y * _localRect.Height);
    }

    public void Translate(Vector2 delta)
    {
        _localRect.Min += delta;
        ComputeWorldTransform();
    }
}
