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

    public float LocalScale => _localTransform.Scale;
    public float WorldScale => WorldTransform.Scale;

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
        set => _worldTransform.Position = value;
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
