using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.Samples;

public class SpriteScene : SceneRootNode
{
    public SpriteScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size) : base(graphics, scaleMode, size)
    {
        SpriteNode spriteNode = new SpriteNode("Sprites/Samples", new Vector2(4, 4), this);
    }
    
    protected override void OnUpdate(GameTime gameTime)
    {
        //DebugDraw.NodeRect(this, Color.BlueViolet);
    }
}