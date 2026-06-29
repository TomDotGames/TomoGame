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
    private int _windowWidth;
    private int _windowHeight;

    /// <summary>The global game instance.</summary>
    public static GameBase? Instance { get; private set; }

    /// <summary>The graphics device manager.</summary>
    protected GraphicsDeviceManager Graphics => _graphicsDeviceManager;

    private Dictionary<string, SceneRootNode> _scenes =  new Dictionary<string, SceneRootNode>();
    private SceneRootNode? _currentRootNode;
    public SceneRootNode? SceneRoot => _currentRootNode;

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
        _currentRootNode?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _currentRootNode?.DrawScene();
        base.Draw(gameTime);
    }

    public Vector2 ViewportToScenePosition(Vector2 viewportPosition)
    {
        if (!Dbg.Verify(_currentRootNode != null))
            return Vector2.Zero;
        
        return new Vector2(viewportPosition.X / _currentRootNode.DrawScale, viewportPosition.Y / _currentRootNode.DrawScale);
    }

    protected void AddScene(string name, SceneRootNode sceneRootNode)
    {
        Dbg.Assert(!_scenes.ContainsKey(name));
        _scenes[name] = sceneRootNode;
    }

    /// <summary>Sets the active scene.</summary>
    protected void SetScene(SceneRootNode sceneRootNode)
    {
        Dbg.Assert(GraphicsDevice != null);
        _currentRootNode = sceneRootNode;
        _currentRootNode.Initialize();
    }
    
    public void SetScene(string name)
    {
        _scenes.TryGetValue(name, out SceneRootNode? sceneRootNode);
        if (!Dbg.Verify(sceneRootNode != null))
            return;
        
        SetScene(sceneRootNode);
    }
}
