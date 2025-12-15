using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using Color = Microsoft.Xna.Framework.Color;


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
        
        private Size m_windowSize;

        protected TomoGame(int width, int height) : base()
        {
            s_instance = this;
            m_windowSize = new Size(width, height);

            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Services.AddService(typeof(GraphicsDeviceManager), _graphicsDeviceManager);
            
            ResourceManager = new ResourceManager(Services);
        }

        protected override void Initialize()
        {
            _graphicsDeviceManager.PreferredBackBufferWidth = m_windowSize.Width;
            _graphicsDeviceManager.PreferredBackBufferHeight = m_windowSize.Height;
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            _graphicsDeviceManager.ApplyChanges();
            
            Content.RootDirectory = "Content";
            
            base.Initialize(); 
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            _rootNode?.Update(deltaTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _rootNode.DrawScene(GraphicsDevice);
            base.Draw(gameTime);
        }

        protected void SetScene(SceneRootNode sceneRootNode)
        {
            _rootNode = sceneRootNode;
            _rootNode.Initialize();
        }
    }
}
