using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

public partial class Node
{
    private Transform _localTransform = Transform.Identity;
    private Transform _worldTransform = Transform.Identity;
    private Transform WorldTransform
    {
        get
        {
            if (_worldTransformIsDirty)
            {
                RecomputeWorldTransform();
            }
            return _worldTransform;
        }
    }
    
    private bool _worldTransformIsDirty = true;

    private Vector2 _size;

    /// <summary>This node's per-axis scale relative to its parent. Composes down the tree, so scaling a node
    /// scales everything under it.</summary>
    public Vector2 LocalScale
    {
        get => _localTransform.Scale;
        set
        {
            _localTransform.Scale = value;
            MarkWorldTransformDirty();
        }
    }

    /// <summary>Sets both axes at once.</summary>
    public float LocalScaleUniform
    {
        set => LocalScale = new Vector2(value, value);
    }

    public Vector2 WorldScale => WorldTransform.Scale;

    /// <summary>This node's rotation in radians, relative to its parent. Composes down the tree, so rotating a
    /// node sweeps everything under it around this node's origin.</summary>
    public float LocalRotation
    {
        get => _localTransform.Rotation;
        set
        {
            _localTransform.Rotation = value;
            MarkWorldTransformDirty();
        }
    }

    public float WorldRotation => WorldTransform.Rotation;

    public Vector2 IntrinsicSize
    {
        get { return _size;  }
        set { _size = value; }
    }
    
    public Vector2 LocalSize
    {
        get { return LocalScale * IntrinsicSize; }
        set { IntrinsicSize = value / LocalScale; }
    }

    public Vector2 WorldSize => WorldScale * _size;
    
    public Vector2 LocalPosition
    {
        get => _localTransform.Position;
        set
        {
            _localTransform.Position = value;
            MarkWorldTransformDirty();
        }
    }

    public Vector2 WorldPosition
    {
        get => WorldTransform.Position;

        // into local space, where position is actually kept. Assigning the cache instead would be undone by
        // the next recompute, and would leave the children behind.
        set => LocalPosition = Parent != null ? Parent.WorldTransform.Inverted().TransformPoint(value) : value;
    }

    public Vector2 OriginUV { get; set; }
    
    public Rect LocalRect
    {
        get
        {
            Vector2 origin = UVToLocalSpace(OriginUV);
            return new Rect(LocalPosition - origin, LocalSize);
        }
    }

    public Rect WorldRect
    {
        get
        {
            Vector2 origin = new Vector2(WorldSize.X * OriginUV.X, WorldSize.Y * OriginUV.Y);
            return new Rect(WorldPosition - origin, WorldSize);
        }
    }
    
    public Node(Vector2 localPosition, Node? parent = null) : this(parent)
    {
        LocalPosition = localPosition;
    }
    
    public Node(Vector2 localPosition, Vector2 size, Node? parent = null) : this(localPosition, parent)
    {
        _size = size;
    }

    public void TranslateInLocalSpace(Vector2 translation)
    {
        _localTransform.Position += translation;
         MarkWorldTransformDirty();
    }

    /// <summary>Sizes this node to exactly wrap its children, keeping its <see cref="OriginUV"/>: the children
    /// are moved as a group so their bounds sit where this node's rect now is. A node that draws nothing of its
    /// own then anchors like a sprite, because its rect and its visible content agree. Children's rotation is
    /// not accounted for.</summary>
    public void SizeToFitChildren()
    {
        if (_children.Count == 0)
            return;

        Vector2 min = new(float.MaxValue);
        Vector2 max = new(float.MinValue);
        foreach (Node child in _children)
        {
            Rect childRect = child.LocalRect;
            min = Vector2.Min(min, childRect.Min);
            max = Vector2.Max(max, childRect.Max);
        }

        IntrinsicSize = max - min;

        // children are placed in our unscaled frame, measured from our origin, where our rect's top left is at
        // -OriginUV * IntrinsicSize
        Vector2 shift = -OriginUV * IntrinsicSize - min;
        foreach (Node child in _children)
        {
            child.TranslateInLocalSpace(shift);
        }
    }

    public Vector2 UVToLocalSpace(Vector2 uv)
    {
        return new Vector2(uv.X * LocalSize.X,  uv.Y * LocalSize.Y);
    }

    /// <summary>A UV point on this node's rect, measured from this node's own origin rather than its top left
    /// corner. That is the space a child's <see cref="LocalPosition"/> lives in, so this is what to anchor a
    /// child against.</summary>
    public Vector2 UVToChildSpace(Vector2 uv)
    {
        return UVToLocalSpace(uv - OriginUV);
    }

    private void MarkWorldTransformDirty()
    {
        _worldTransformIsDirty = true;
        foreach (Node child in _children)
        {
            child.MarkWorldTransformDirty();
        }
    }

    private void RecomputeWorldTransform()
    {
        _worldTransform = Parent != null ? _localTransform.AppliedTo(Parent.WorldTransform) : _localTransform;
        _worldTransformIsDirty = false;
    }
}
