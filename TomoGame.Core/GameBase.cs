using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    /// <summary>How much the window was scaled up for the display it opened on. 1 on an ordinary display.</summary>
    public float DisplayScaleFactor { get; private set; } = 1f;

    /// <summary>Creates the game with a window of the given size. The size is in logical pixels: on a high
    /// density display the window is scaled up to match, so it comes out the physical size you asked for.</summary>
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
        
        DisplayScaleFactor = ResolveWindowScale();
        _graphicsDeviceManager.PreferredBackBufferWidth = (int)MathF.Round(_windowWidth * DisplayScaleFactor);
        _graphicsDeviceManager.PreferredBackBufferHeight = (int)MathF.Round(_windowHeight * DisplayScaleFactor);
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        _graphicsDeviceManager.ApplyChanges();

        Log.Info($"Window {_windowWidth}x{_windowHeight} at display scale {DisplayScaleFactor:0.##} " +
                 $"-> {_graphicsDeviceManager.PreferredBackBufferWidth}x{_graphicsDeviceManager.PreferredBackBufferHeight}");

        base.Initialize();
    }

    /// <summary>The display's scale, held back so the scaled window still fits on the display. Scaling a
    /// window off the edge of the screen is worse than not scaling it at all.</summary>
    private float ResolveWindowScale()
    {
        float scale = DisplayScale.Get();

        DisplayMode display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        float widthLimit = (float)display.Width / _windowWidth;
        float heightLimit = (float)display.Height / _windowHeight;

        return MathF.Max(1f, MathF.Min(scale, MathF.Min(widthLimit, heightLimit)));
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
