using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;


namespace TomoGame.Core
{
    public class TomoGame : Game
    {
        private static TomoGame s_instance;
        public static TomoGame Instance => s_instance;
        
        private SceneRootNode _rootNode;
        
        public ResourceManager ResourceManager { get; }

        public GraphicsDeviceManager GraphicsDeviceManager => _graphicsDeviceManager;
        private GraphicsDeviceManager _graphicsDeviceManager;

        protected TomoGame(int width, int height) : base()
        {
            s_instance = this;
            
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.PreferredBackBufferWidth = width;
            _graphicsDeviceManager.PreferredBackBufferHeight = height;
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;

            Services.AddService(typeof(GraphicsDeviceManager), _graphicsDeviceManager);
            Content.RootDirectory = "Content";
            ResourceManager = new ResourceManager(Services);
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            _rootNode?.Update(deltaTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Matrix baseTransform = Matrix.Identity;
            baseTransform *= Matrix.CreateScale(10f);
            
            GraphicsDevice.Clear(Color.ForestGreen);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                baseTransform);
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
