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
            FixedHeight,
            FixedWidth
        }

        public float ScaleFactor => m_flScaleFactor;
        private float m_flScaleFactor;

        public Node RootNode => m_rootNode;
        private Node m_rootNode;

        public GraphicsDevice Graphics => m_graphics.GraphicsDevice;
        protected GraphicsDeviceManager m_graphics;

        public Vector2 Size => m_size;
        private Vector2 m_size;

        public Scene(GraphicsDeviceManager graphics, ESceneScaleMode eScaleMode, int nSize)
        {
            Debug.Assert(nSize > 0);

            // eventually we will want to be able to change the window size
            float flWindowSize = eScaleMode == ESceneScaleMode.FixedHeight ? graphics.PreferredBackBufferHeight : graphics.PreferredBackBufferWidth;
            Debug.Assert(flWindowSize > 0);

            m_flScaleFactor = flWindowSize / nSize;
            float flWidth = m_flScaleFactor * graphics.PreferredBackBufferWidth;
            float flHeight = m_flScaleFactor * graphics.PreferredBackBufferHeight;
            m_size = new Vector2(flWidth, flHeight);
            
            m_rootNode = new Node(this);
            m_graphics = graphics;
        }

        public virtual void Initialize()
        {
            RootNode.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            float flDeltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond;
            RootNode.Update(flDeltaTime);
        }

        public void Render(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Debug.Assert(spriteBatch != null);

            float flDeltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond; // this is wrong?
            RootNode.Render(flDeltaTime, spriteBatch);
        }

        public Vector2 WindowToSceneCoords(Vector2 vWindowCoords)
        {
            return vWindowCoords / m_flScaleFactor;
        }

        public Rect SceneToWindowRect(Rect rectScene)
        {
            Rect rectWindow = new Rect(rectScene.Origin * m_flScaleFactor, rectScene.Size * m_flScaleFactor, rectScene.Origin);
            return rectWindow;
        }
    }
}
