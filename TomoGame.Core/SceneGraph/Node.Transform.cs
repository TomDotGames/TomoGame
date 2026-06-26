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
    
    // todo: need to take into account the origin here!
    public Rect LocalRect => new Rect(LocalPosition, LocalSize);
    public Rect WorldRect => new Rect(WorldPosition, WorldSize);
    
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
