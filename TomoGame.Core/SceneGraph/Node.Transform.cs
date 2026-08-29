using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

[LayoutNode("Transform")]
public partial class Node
{
    private Rect _localRect; // the rect of this transform with intrinsic size, in parent's intrinsic space

    public Rect WorldRect => _worldRect;
    public Vector2 WorldPosition => _worldRect.Min;
    /// <summary>The world-space position of this node's <see cref="Anchor"/> point.</summary>
    public Vector2 WorldAnchorPosition => _worldRect.Min + (_worldRect.Size * _anchor);
    private Rect _worldRect;

    private Vector2 _localScale = Vector2.One;

    public Vector2 WorldScale => _worldScale;
    private Vector2 _worldScale = Vector2.One;

    /// <summary>This node's rotation, in radians, relative to its parent.</summary>
    public float WorldRotation => _worldRotation;
    private float _worldRotation;
    private float _localRotation;

    /// <summary>The pivot point of this node within its own local rect, in UV space (0,0 is top-left, 1,1 is bottom-right). Defaults to bottom-left.</summary>
    public Vector2 Anchor
    {
        get => _anchor;
        set
        {
            _anchor = value;
            ReapplyPositionConstraint();
            ComputeWorldTransform();
        }
    }
    private Vector2 _anchor = new(0f, 1f);

    private Vector2? _parentAnchorUV;
    private Vector2 _offset;

    public Node(Vector2 localPosition, Vector2 size, Node? parent = null) : this(parent)
    {
        _localRect.Min = localPosition;
        _localRect.Size = size;
        ComputeWorldTransform();
    }
    
    public virtual void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? size = element.Attribute("size");
        if (size != null)
        {
            string[] tokens = size.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (Dbg.Verify(tokens.Length == 2))
            {
                SetIntrinsicSize(float.Parse(tokens[0]), float.Parse(tokens[1]));
            }
        }

        XAttribute? scale = element.Attribute("scale");
        if (scale != null)
        {
            string[] tokens = scale.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (Dbg.Verify(tokens.Length == 2))
            {
                SetLocalScale(new Vector2(float.Parse(tokens[0]), float.Parse(tokens[1])));
            }
        }

        XAttribute? rotation = element.Attribute("rotation");
        if (rotation != null)
        {
            SetLocalRotation(MathHelper.ToRadians(float.Parse(rotation.Value)));
        }

        XAttribute? anchor = element.Attribute("anchor");
        if (anchor != null)
        {
            Anchor = AnchorPositionFromString(anchor.Value);
        }

        XAttribute? pos = element.Attribute("pos");
        if (pos != null)
        {
            string[] tokens = pos.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!Dbg.Verify(tokens.Length == 4))
                return;

            Anchor = AnchorPositionFromString(tokens[0]);
            Vector2 parentAnchorUV = AnchorPositionFromString(tokens[1]);
            Vector2 offset = new Vector2(float.Parse(tokens[2]), float.Parse(tokens[3]));

            SetPositionInParentSpace(parentAnchorUV, offset);
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
        foreach (Node child in _children)
        {
            child.ReapplyPositionConstraint();
        }
        ComputeWorldTransform();
    }

    public void SetLocalScale(float scale)
    {
        SetLocalScale(new Vector2(scale, scale));
    }

    public void SetLocalScale(Vector2 scale)
    {
        _localScale = scale;
        ComputeWorldTransform();
    }

    public void SetLocalRotation(float radians)
    {
        _localRotation = radians;
        ComputeWorldTransform();
    }

    /// <summary>Positions this node's <see cref="Anchor"/> point at the given anchor point of the parent's rect, plus an offset. The constraint is re-applied automatically whenever this node's anchor or the parent's size changes.</summary>
    public void SetPositionInParentSpace(Vector2 parentAnchorUV, Vector2 offset)
    {
        _parentAnchorUV = parentAnchorUV;
        _offset = offset;
        ReapplyPositionConstraint();
        ComputeWorldTransform();
    }

    private bool ReapplyPositionConstraint()
    {
        if (_parentAnchorUV is not Vector2 parentAnchorUV)
            return false;

        Vector2 parentAnchor = Parent?.UVToIntrinsicPosition(parentAnchorUV) ?? Vector2.Zero;
        Vector2 selfAnchor = UVToIntrinsicPosition(_anchor);
        _localRect.Min = (parentAnchor - selfAnchor) + _offset;
        return true;
    }

    private void ComputeWorldTransform()
    {
        Vector2 parentWorldScale = Parent != null ? Parent.WorldScale : Vector2.One;
        Vector2 parentWorldPosition = Parent != null ? Parent.WorldPosition : Vector2.Zero;
        float parentWorldRotation = Parent?.WorldRotation ?? 0f;

        _worldScale = _localScale * parentWorldScale;
        _worldRotation = _localRotation + parentWorldRotation;
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
