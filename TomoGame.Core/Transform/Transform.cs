using Microsoft.Xna.Framework;

namespace TomoGame.Core;

public struct Transform
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    /// <summary>Scale per axis. Composing a non-uniform scale with a rotation is a shear, which a
    /// position/rotation/scale triple cannot hold, so the axes compose independently of rotation: exact while
    /// a scaled node is unrotated, an approximation once it turns.</summary>
    public Vector2 Scale { get; set; } = Vector2.One;


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
        outTransform.Scale = Vector2.One / Scale;
        outTransform.Rotation = -Rotation;
        outTransform.Position = -outTransform.TransformPoint(Position);
        return outTransform;
    }
}