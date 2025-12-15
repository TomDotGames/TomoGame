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
        
        private float _sceneDrawScale = 1;
        
        public SceneRootNode(GraphicsDeviceManager graphics, SceneScaleMode eScaleMode, int nSize)
        {
            Debug.Assert(nSize > 0);

            float flWindowSize = eScaleMode == SceneScaleMode.FixedHeight ? graphics.PreferredBackBufferHeight : graphics.PreferredBackBufferWidth;
            Debug.Assert(flWindowSize > 0);

            _sceneDrawScale = flWindowSize / nSize;
            float width = _sceneDrawScale * graphics.PreferredBackBufferWidth;
            float height = _sceneDrawScale * graphics.PreferredBackBufferHeight;
            SetLocalSize(width, height);
        }

        public void DrawScene(GraphicsDevice graphics)
        {
            Matrix baseTransform = Matrix.Identity;
            baseTransform *= Matrix.CreateScale(_sceneDrawScale);
            
            graphics.Clear(Color.ForestGreen);
            SpriteBatch spriteBatch = new SpriteBatch(graphics);
            spriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                baseTransform);
            
            base.Draw(spriteBatch);
            
            spriteBatch.End();
        }
    }
}
