using Microsoft.Xna.Framework;

namespace TomoGame.Core.Input;

public class RectPointable : Pointable
{
    public Rect Rect;
    
    public override bool IsPointInside(Vector2 point)
    {
        return Rect.Contains(point);
    }
}