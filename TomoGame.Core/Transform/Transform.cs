using Microsoft.Xna.Framework;

namespace TomoGame.Core;

public struct Transform
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public float Scale { get; set; } = 1f;
    
    public static Transform Identity { get; } = new Transform();

    public Transform()
    {
    }

    public Vector2 TransformPoint(Vector2 position)
    {
        position *= Scale;
        position.Rotate(Rotation);
        position += Position;
        return position;
    }
    
    public Transform AppliedTo(Transform other)
    {
        Transform outTransform = new Transform();
        outTransform.Scale = other.Scale * Scale;
        outTransform.Rotation = other.Rotation + Rotation;
        outTransform.Position = other.TransformPoint(Position);
        return outTransform;
    }
    
    public Transform RelativeTo(Transform other)
    {
        Transform otherInverted = other.Inverted();
        return this.AppliedTo(otherInverted);
    }

    public Transform Inverted()
    {
        Transform outTransform = new Transform();
        outTransform.Scale = 1 / Scale; 
        outTransform.Rotation = -Rotation;
        outTransform.Position = -outTransform.TransformPoint(Position);
        return outTransform;
    }
}