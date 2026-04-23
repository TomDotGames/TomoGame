using Microsoft.Xna.Framework;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Resources;

namespace TomoGame.Core;

/// <summary>Base class for a TomoGame game. Extend this to implement your game.</summary>
public class GameBase : Game
{
    /// <summary>The global game instance.</summary>
    public static GameBase? Instance { get; private set; }

    private GraphicsDeviceManager _graphicsDeviceManager;
    private ResourceManager _resourceManager;

    /// <summary>The graphics device manager.</summary>
    protected GraphicsDeviceManager Graphics => _graphicsDeviceManager;

    private SceneRootNode? _rootNode;

    /// <summary>The root node of the active scene.</summary>
    public SceneRootNode? SceneRoot => _rootNode;

    private int _windowWidth;
    private int _windowHeight;

    protected GameBase(int width, int height) : base()
    {
        Dbg.Assert(Instance == null);
        Instance = this;
        _windowWidth = width;
        _windowHeight = height;
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void Initialize()
    {
        Log.InitOutputFile("game.log");

        _resourceManager = new ResourceManager(Services);
        
        _graphicsDeviceManager.PreferredBackBufferWidth = _windowWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = _windowHeight;
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        _graphicsDeviceManager.ApplyChanges();

        Content.RootDirectory = "Content";

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        _rootNode?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _rootNode?.DrawScene();
        base.Draw(gameTime);
    }

    /// <summary>Sets the active scene.</summary>
    protected void SetScene(SceneRootNode sceneRootNode)
    {
        Dbg.Assert(GraphicsDevice != null);
        _rootNode = sceneRootNode;
        _rootNode.Initialize();
    }
}
