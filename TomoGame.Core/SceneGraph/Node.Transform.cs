using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

[LayoutNode("Transform")]
public partial class Node
{
    private Rect _localRect; // the rect of this transform with intrinsic size, in parent's intrinsic space

    /// <summary>This node's rect in world space. <see cref="Rect"/> is axis-aligned, so when this node or any of its
    /// ancestors is rotated, Min is the rotated position of the local rect's top-left corner rather than the corner of
    /// a bounding box. Use <see cref="WorldAnchorPosition"/> and <see cref="WorldRotation"/> for the exact placement.</summary>
    public Rect WorldRect => _worldRect;
    public Vector2 WorldPosition => _worldRect.Min;
    private Rect _worldRect;

    /// <summary>The world-space position of this node's <see cref="Anchor"/> point. This is the pivot the node rotates
    /// and scales about, and the point that stays fixed when its rotation or scale changes.</summary>
    public Vector2 WorldAnchorPosition => _worldAnchorPosition;
    private Vector2 _worldAnchorPosition;

    /// <summary>This node's intrinsic (unscaled) size, in its parent's space.</summary>
    public Vector2 IntrinsicSize => _localRect.Size;

    private Vector2 _localScale = Vector2.One;

    public Vector2 WorldScale => _worldScale;
    private Vector2 _worldScale = Vector2.One;

    /// <summary>This node's accumulated rotation, in radians, including any rotation inherited from its ancestors.</summary>
    public float WorldRotation => _worldRotation;
    private float _worldRotation;
    private float _localRotation;

    /// <summary>Where this node draws relative to the rest of the graph. Higher draws later, so on top.
    /// It accumulates down the tree like the rest of the transform, so raising a node's order lifts its whole
    /// subtree, and nodes sharing an order keep the graph's own order: parents before children, siblings as
    /// they were added.</summary>
    public float ZOrder
    {
        get => _zOrder;
        set
        {
            if (Math.Abs(_zOrder - value) < float.Epsilon)
                return;

            _zOrder = value;
            ComputeWorldTransform();
        }
    }
    private float _zOrder;

    /// <summary>This node's draw order including everything inherited from its ancestors.</summary>
    public float WorldZOrder => _worldZOrder;
    private float _worldZOrder;

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
            if (Dbg.Verify(tokens.Length == 2, "size expects 2 values, eg: size=\"32 32\""))
            {
                SetIntrinsicSize(ParseFloat(tokens[0]), ParseFloat(tokens[1]));
            }
        }

        XAttribute? scale = element.Attribute("scale");
        if (scale != null)
        {
            string[] tokens = scale.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (Dbg.Verify(tokens.Length == 2, "scale expects 2 values, eg: scale=\"2 2\""))
            {
                SetLocalScale(new Vector2(ParseFloat(tokens[0]), ParseFloat(tokens[1])));
            }
        }

        XAttribute? rotation = element.Attribute("rotation");
        if (rotation != null)
        {
            SetLocalRotation(MathHelper.ToRadians(ParseFloat(rotation.Value)));
        }

        XAttribute? anchor = element.Attribute("anchor");
        XAttribute? pos = element.Attribute("pos");

        // pos carries its own self-anchor in its first token, so the two would fight over Anchor.
        Dbg.Verify(anchor == null || pos == null, "anchor is ignored when pos is also specified; pos sets the anchor itself");

        if (anchor != null && pos == null)
        {
            Anchor = AnchorPositionFromString(anchor.Value);
        }

        if (pos != null)
        {
            string[] tokens = pos.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!Dbg.Verify(tokens.Length == 4, "pos expects 4 values, eg: pos=\"cc cc 0 0\""))
                return;

            Anchor = AnchorPositionFromString(tokens[0]);
            Vector2 parentAnchorUV = AnchorPositionFromString(tokens[1]);
            Vector2 offset = new Vector2(ParseFloat(tokens[2]), ParseFloat(tokens[3]));

            SetPositionInParentSpace(parentAnchorUV, offset);
        }
    }

    /// <summary>Parses a float from a layout file. Layout files are authored with '.' as the decimal separator regardless of the machine's locale.</summary>
    private static float ParseFloat(string value)
    {
        return float.Parse(value, CultureInfo.InvariantCulture);
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

        // this node's own anchor point moved with its size, and so did the anchor points its children measure against
        ReapplyPositionConstraint();
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

    /// <summary>Positions this node's <see cref="Anchor"/> point at the given anchor point of the parent's rect, plus an offset. The constraint is re-applied automatically whenever this node's anchor or size, or the parent's size, changes.</summary>
    public void SetPositionInParentSpace(Vector2 parentAnchorUV, Vector2 offset)
    {
        _parentAnchorUV = parentAnchorUV;
        _offset = offset;
        ReapplyPositionConstraint();
        ComputeWorldTransform();
    }

    private void ReapplyPositionConstraint()
    {
        if (_parentAnchorUV is not Vector2 parentAnchorUV)
            return;

        Vector2 parentAnchor = Parent?.UVToIntrinsicPosition(parentAnchorUV) ?? Vector2.Zero;
        Vector2 selfAnchor = UVToIntrinsicPosition(_anchor);
        _localRect.Min = (parentAnchor - selfAnchor) + _offset;
    }

    private void ComputeWorldTransform()
    {
        Vector2 anchorLocal = UVToIntrinsicPosition(_anchor); // this node's pivot, in its own local rect space

        if (Parent != null)
        {
            _worldScale = _localScale * Parent._worldScale;
            _worldRotation = _localRotation + Parent._worldRotation;
            _worldZOrder = _zOrder + Parent._worldZOrder;

            // place our pivot relative to the parent's pivot, then carry it through the parent's scale and rotation
            Vector2 pivotInParent = _localRect.Min + anchorLocal;
            Vector2 parentPivot = Parent.UVToIntrinsicPosition(Parent._anchor);
            Vector2 offsetFromParentPivot = (pivotInParent - parentPivot) * Parent._worldScale;
            _worldAnchorPosition = Parent._worldAnchorPosition + Rotate(offsetFromParentPivot, Parent._worldRotation);
        }
        else
        {
            _worldScale = _localScale;
            _worldRotation = _localRotation;
            _worldZOrder = _zOrder;
            _worldAnchorPosition = _localRect.Min + anchorLocal;
        }

        _worldRect.Size = _localRect.Size * _worldScale;
        _worldRect.Min = _worldAnchorPosition - Rotate(anchorLocal * _worldScale, _worldRotation);

        foreach (Node child in _children)
        {
            child.ComputeWorldTransform();
        }
    }

    private static Vector2 Rotate(Vector2 v, float radians)
    {
        if (radians == 0f)
            return v;

        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);
        return new Vector2((v.X * cos) - (v.Y * sin), (v.X * sin) + (v.Y * cos));
    }

    private Vector2 UVToIntrinsicPosition(Vector2 UV)
    {
        return new Vector2(UV.X * _localRect.Width, UV.Y * _localRect.Height);
    }

    public void Translate(Vector2 delta)
    {
        // fold the move into the anchor offset so an anchored node stays anchored instead of
        // being snapped back the next time its constraint is re-applied
        if (_parentAnchorUV != null)
            _offset += delta;

        _localRect.Min += delta;
        ComputeWorldTransform();
    }
}
