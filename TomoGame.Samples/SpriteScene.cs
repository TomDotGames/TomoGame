using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.Samples;

/// <summary>Sprites, including animated ones. The scene is a layout file; the only thing here is the dog's
/// patrol, which is behaviour and so cannot be data.</summary>
public class SpriteScene : SceneRootNode
{
    private const float MoveSpeed = 10f;
    private const float LeftLimit = 5f;
    private const float RightLimit = 30f;

    private readonly SpriteNode? _dog;
    private bool _dogGoingRight = true;

    public SpriteScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        LayoutNode layout = new LayoutNode("UI/SpriteSceneLayout.xml", this);

        _dog = layout.FindNode("dog") as SpriteNode;
        Dbg.Verify(_dog, "sprite layout has no node named 'dog'");
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        if (_dog == null)
            return;

        float moveAmount = Time.TickSeconds * MoveSpeed;
        _dog.FlipX = !_dogGoingRight;
        _dog.TranslateInLocalSpace(new Vector2(_dogGoingRight ? moveAmount : -moveAmount, 0f));

        if (_dogGoingRight && _dog.WorldPosition.X > RightLimit)
            _dogGoingRight = false;
        else if (!_dogGoingRight && _dog.WorldPosition.X < LeftLimit)
            _dogGoingRight = true;
    }
}
