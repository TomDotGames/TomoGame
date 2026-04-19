using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.SceneGraph;

/// <summary>The root node of the scene graph. Manages the render pipeline and maps scene space to window space.</summary>
public class SceneRootNode : TransformNode
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
        SetSize(width, height);
    }

    protected override void OnInitialize()
    {
        _spriteBatch = new SpriteBatch(GameBase.Instance!.GraphicsDevice);
    }

    internal void DrawScene()
    {
        GraphicsDevice graphicsDevice = GameBase.Instance!.GraphicsDevice;
        Matrix baseTransform = Matrix.CreateScale(_sceneDrawScale);

        graphicsDevice.Clear(Color.ForestGreen);
        _spriteBatch.Begin(
            SpriteSortMode.FrontToBack,
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
