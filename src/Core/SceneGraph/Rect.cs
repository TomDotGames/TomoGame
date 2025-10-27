
using Microsoft.Xna.Framework;
using System;

namespace TomoGame.Core.SceneGraph
{
    public class Rect
    {
        public static readonly Vector2 UVTopLeft        = new Vector2(0.0f, 0.0f);
        public static readonly Vector2 UVTopCenter      = new Vector2(0.5f, 0.0f);
        public static readonly Vector2 UVTopRight       = new Vector2(1.0f, 0.0f);
        public static readonly Vector2 UVCenterLeft     = new Vector2(0.0f, 0.5f);
        public static readonly Vector2 UVCenter         = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 UVCenterRight    = new Vector2(1.0f, 0.5f);
        public static readonly Vector2 UVBottomLeft     = new Vector2(0.0f, 1.0f);
        public static readonly Vector2 UVBottomCenter   = new Vector2(0.5f, 1.0f);
        public static readonly Vector2 UVBottomRight    = new Vector2(1.0f, 1.0f);

        public Vector2 Position;
        public Vector2 Size;
        public Vector2 OriginUV;

        public Rect() { }

        public Rect(Vector2 vPosition, Vector2 vSize, Vector2 vOriginUV)
        {
            Position = vPosition;
            Size = vSize;
            OriginUV = vOriginUV;
        }

        public void Expand(Vector2 vSize)
        {
            Size.X = MathF.Max(Size.X, vSize.X);
            Size.Y = MathF.Max(Size.Y, vSize.Y);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }

        public Vector2 UVToLocalCoords(Vector2 vUV)
        {
            return (vUV - OriginUV) * Size;
        }

        public Vector2 LocalToWorldCoords(Vector2 vLocal)
        {
            return Position + vLocal;
        }

        public Vector2 UVToWorldCoords(Vector2 vUV)
        {
            return LocalToWorldCoords(UVToLocalCoords(vUV));
        }
    }
}
