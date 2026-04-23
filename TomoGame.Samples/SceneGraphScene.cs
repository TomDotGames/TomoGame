using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Samples;

public class SceneGraphScene : SceneRootNode
{
    public SceneGraphScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size) : base(graphics, scaleMode, size)
    {
        TransformNode anotherNode = new TransformNode(new Vector2(10, 20), new Vector2(4, 8), this);
    }
    
    protected override void OnUpdate(GameTime gameTime)
    {
        DebugDraw.NodeRect(this, Color.BlueViolet);
    }
}