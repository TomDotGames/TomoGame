using Microsoft.Xna.Framework;
using TomoGame.Core.Input;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Resources;

namespace TomoGame.Core;

/// <summary>Base class for a TomoGame game. Extend this to implement your game.</summary>
public class GameBase : Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private ResourceManager? _resourceManager;
    private InputManager? _inputManager;
    private SceneRootNode? _rootNode;
    private SceneRootNode? _pendingScene;
    private int _windowWidth;
    private int _windowHeight;

    /// <summary>The global game instance.</summary>
    public static GameBase? Instance { get; private set; }

    /// <summary>The graphics device manager.</summary>
    public GraphicsDeviceManager Graphics => _graphicsDeviceManager;

    /// <summary>The root node of the active scene.</summary>
    public SceneRootNode? SceneRoot => _rootNode;

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
        _inputManager = new InputManager(this);
        
        _graphicsDeviceManager.PreferredBackBufferWidth = _windowWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = _windowHeight;
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        _graphicsDeviceManager.ApplyChanges();

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Set(gameTime);
        ApplyPendingScene();
        _rootNode?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _rootNode?.DrawScene();
        base.Draw(gameTime);
    }

    /// <summary>Makes a scene the active one, replacing and destroying whatever was there. The swap happens
    /// at the start of the next tick rather than immediately, so it is safe to call from anywhere - including
    /// a pointer handler, which is raised from inside the outgoing scene's own graph.</summary>
    public void SetScene(SceneRootNode sceneRootNode)
    {
        Dbg.Assert(GraphicsDevice != null);
        _pendingScene = sceneRootNode;
    }

    private void ApplyPendingScene()
    {
        if (_pendingScene == null)
            return;

        // destroying the outgoing scene runs OnDestroy down the whole graph, which is what releases its
        // pointer registrations and its sprite batch
        _rootNode?.Destroy();

        _rootNode = _pendingScene;
        _pendingScene = null;
        _rootNode.Initialize();
    }
}
