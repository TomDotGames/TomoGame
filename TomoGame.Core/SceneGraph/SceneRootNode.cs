using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.SceneGraph;

/// <summary>The root node of the scene graph. Manages the render pipeline and maps scene space to window space.</summary>
public class SceneRootNode : Node
{
    /// <summary>Determines which axis is fixed when scaling the scene to the window.</summary>
    public enum SceneScaleMode
    {
        /// <summary>The scene height is fixed; width scales with the window aspect ratio.</summary>
        FixedHeight,
        /// <summary>The scene width is fixed; height scales with the window aspect ratio.</summary>
        FixedWidth
    }

    private readonly float _sceneDrawScale;
    private SpriteBatch _spriteBatch = null!;

    public SceneRootNode(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(Vector2.Zero, Vector2.Zero)
    {
        Dbg.Assert(size > 0);

        Viewport viewport = graphics.GraphicsDevice.Viewport;
        float windowSize = scaleMode == SceneScaleMode.FixedHeight ? viewport.Height : viewport.Width;
        Dbg.Assert(windowSize > 0);

        _sceneDrawScale = windowSize / size;
        float width = viewport.Width / _sceneDrawScale;
        float height = viewport.Height / _sceneDrawScale;
        SetIntrinsicSize(width, height);
    }

    /// <summary>Converts a position in window pixels to scene units. Pointer input arrives in window pixels,
    /// while every node's rect is in scene units, so hit tests must go through here.</summary>
    public Vector2 ScreenToScene(Vector2 screenPosition)
    {
        return screenPosition / _sceneDrawScale;
    }

    protected override void OnInitialize()
    {
        _spriteBatch = new SpriteBatch(GameBase.Instance!.GraphicsDevice);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _spriteBatch?.Dispose();
    }

    internal void DrawScene()
    {
        GraphicsDevice graphicsDevice = GameBase.Instance!.GraphicsDevice;
        Matrix baseTransform = Matrix.CreateScale(_sceneDrawScale);

        graphicsDevice.Clear(Color.ForestGreen);
        // Deferred draws in submission order, which is the scene graph's own order: parents before children,
        // siblings in the order they were added. FrontToBack sorted on a layerDepth every node left at 0,
        // so equal keys came out in an arbitrary order and children could land behind their parents.
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default,
            RasterizerState.CullNone,
            null,
            baseTransform);

        Draw(_spriteBatch);

        _spriteBatch.End();
    }
}
