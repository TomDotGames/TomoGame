using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using TomoGame.Core.SceneGraph;


namespace TomoGame.Core
{
    public class TomoGame : Game
    {
        private static TomoGame s_instance;
        public static TomoGame Instance => s_instance;
 
        private GraphicsDeviceManager m_graphicsDeviceManager;
        protected GraphicsDeviceManager Graphics => m_graphicsDeviceManager;

        private Scene m_activeScene;

        public TomoGame()
        {
            s_instance = this;

            m_graphicsDeviceManager = new GraphicsDeviceManager(this);

            Services.AddService(typeof(GraphicsDeviceManager), m_graphicsDeviceManager);
            Content.RootDirectory = "Content";
            m_graphicsDeviceManager.PreferredBackBufferWidth = 240 * 6;
            m_graphicsDeviceManager.PreferredBackBufferHeight = 240 * 4;
            m_graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Update(GameTime gameTime)
        {
            m_activeScene?.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MonoGameOrange);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            m_activeScene?.Render(gameTime, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void SetScene(Scene scene)
        {
            m_activeScene = scene;
            m_activeScene.Initialize();
        }
    }
}
