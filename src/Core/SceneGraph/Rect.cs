
using Microsoft.Xna.Framework;
using System;

namespace TomoGame.Core.SceneGraph
{
    public struct Rect
    {
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Origin;

        public Rect(Vector2 vPosition, Vector2 vSize, Vector2 vOrigin)
        {
            Position = vPosition;
            Size = vSize;
            Origin = vOrigin;
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
    }
}
