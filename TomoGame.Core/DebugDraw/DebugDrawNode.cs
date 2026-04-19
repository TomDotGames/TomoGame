using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core;

internal class DebugDrawNode(Node? parent = null) : Node(parent)
{
    private readonly struct LineDrawRequest(Line line, Color color, float thickness)
    {
        public Line DrawLine { get; } = line;
        public Color DrawColor { get; } = color;
        public float DrawThickness { get; } = thickness;
    }

    private readonly List<LineDrawRequest> _lineDrawRequests = [];
    private Texture2D? _pixelTexture;

    protected override void OnInitialize()
    {
        _pixelTexture = new Texture2D(GameBase.Instance!.GraphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
    }

    public void AddLine(Vector2 start, Vector2 end, Color color, float thickness)
    {
        _lineDrawRequests.Add(new LineDrawRequest(new Line(start, end), color, thickness));
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        foreach (LineDrawRequest request in _lineDrawRequests)
        {
            DrawLine(spriteBatch, request);
        }
        _lineDrawRequests.Clear();
    }

    private void DrawLine(SpriteBatch spriteBatch, LineDrawRequest request)
    {
        if (!Dbg.Verify(_pixelTexture != null))
            return;

        Vector2 scale = new Vector2(request.DrawLine.Length, request.DrawThickness);
        spriteBatch.Draw(_pixelTexture, request.DrawLine.Start, null, request.DrawColor, request.DrawLine.Angle, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}