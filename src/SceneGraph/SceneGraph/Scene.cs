using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace TomoGame.Core.SceneGraph
{
    public class Scene
    {
        public enum ESceneScaleMode
        {
            Fixed_Height,
            Fixed_Width
        }

        public float ScaleFactor => m_flScaleFactor;
        private float m_flScaleFactor;

        public Node RootNode => m_rootNode;
        private Node m_rootNode;

        public GraphicsDevice Graphics => m_graphics.GraphicsDevice;
        protected GraphicsDeviceManager m_graphics;

        public Scene(GraphicsDeviceManager graphics, ESceneScaleMode eScaleMode, int nSize)
        {
            Debug.Assert(nSize > 0);

            // eventually we will want to be able to change the window size
            float flWindowSize = eScaleMode == ESceneScaleMode.Fixed_Height ? graphics.PreferredBackBufferHeight : graphics.PreferredBackBufferWidth;
            Debug.Assert(flWindowSize > 0);

            m_flScaleFactor = flWindowSize / nSize;
            m_rootNode = new Node(null, this);
            m_graphics = graphics;
        }

        public virtual void Initialize()
        {
            RootNode.Initialize();
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Debug.Assert(spriteBatch != null);

            float flDeltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond; // this is wrong?
            RootNode.Draw(flDeltaTime, spriteBatch);
        }

        public virtual void Update(GameTime gameTime)
        {
            float flDeltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond;
            RootNode.Update(flDeltaTime);
        }

        public Vector2 WindowToSceneCoords(Vector2 vWindowCoords)
        {
            return vWindowCoords / m_flScaleFactor;
        }
    }
}
