using Microsoft.Xna.Framework;

namespace TomoGame.Core.Tweening;

/// <summary>Easing curves. Each comes in a float form shaping t, and a Vector2 form that eases between two
/// points, which is the shape <see cref="Tween{T}"/> takes.</summary>
public static class Ease
{
    public static Vector2 Linear(Vector2 start, Vector2 end, float t)
    {
        return Vector2.Lerp(start, end, t);
    }

    public static float InOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;
    }

    public static Vector2 InOutQuad(Vector2 start, Vector2 end, float t)
    {
        return Linear(start, end, InOutQuad(t));
    }

    public static float InCubic(float t)
    {
        return t * t * t;
    }

    public static Vector2 InCubic(Vector2 start, Vector2 end, float t)
    {
        return Linear(start, end, InCubic(t));
    }

    public static float OutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
            return n1 * t * t;

        if (t < 2f / d1)
            return n1 * (t -= 1.5f / d1) * t + 0.75f;

        if (t < 2.5f / d1)
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;

        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }

    public static Vector2 OutBounce(Vector2 start, Vector2 end, float t)
    {
        return Linear(start, end, OutBounce(t));
    }
}
