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
    private int _windowWidth;
    private int _windowHeight;

    /// <summary>The global game instance.</summary>
    public static GameBase? Instance { get; private set; }

    /// <summary>The graphics device manager.</summary>
    protected GraphicsDeviceManager Graphics => _graphicsDeviceManager;

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

    public Vector2 ViewportToScenePosition(Vector2 viewportPosition)
    {
        if (!Dbg.Verify(_rootNode != null))
            return Vector2.Zero;
        
        return new Vector2(viewportPosition.X / _rootNode.DrawScale, viewportPosition.Y / _rootNode.DrawScale);
    }
}
