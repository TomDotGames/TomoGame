using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.SceneGraph;


namespace TomoGame.Core
{
    public class TomoGame : Game
    {
        private static TomoGame s_instance;
        public static TomoGame Instance => s_instance;
        
        private SceneRootNode _rootNode;

        public TomoGame(int width, int height) : base()
        {
            s_instance = this;
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);

            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);
            Content.RootDirectory = "Content";
            graphicsDeviceManager.PreferredBackBufferWidth = width;
            graphicsDeviceManager.PreferredBackBufferHeight = height;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            _rootNode?.Update(deltaTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MonoGameOrange);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            _rootNode?.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void SetScene(SceneRootNode sceneRootNode)
        {
            _rootNode = sceneRootNode;
            _rootNode.Initialize();
        }
    }
}
