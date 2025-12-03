using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace TomoGame.Core.SceneGraph
{
    public class SceneRootNode : Node
    {
        public enum SceneScaleMode
        {
            FixedHeight,
            FixedWidth
        }
        
        public SceneRootNode(GraphicsDeviceManager graphics, SceneScaleMode eScaleMode, int nSize)
        {
            Debug.Assert(nSize > 0);

            float flWindowSize = eScaleMode == SceneScaleMode.FixedHeight ? graphics.PreferredBackBufferHeight : graphics.PreferredBackBufferWidth;
            Debug.Assert(flWindowSize > 0);

            float scale = flWindowSize / nSize;
            float width = scale * graphics.PreferredBackBufferWidth;
            float height = scale * graphics.PreferredBackBufferHeight;
            SetLocalSize(width, height);
        }
    }
}
