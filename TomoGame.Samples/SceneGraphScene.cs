using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Samples;

public class SceneGraphScene : SceneRootNode
{
    public SceneGraphScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size) : base(graphics, scaleMode, size)
    {
        Node anotherNode = new Node(new Vector2(10, 20), this);
    }
    
    protected override void OnUpdate(GameTime gameTime)
    {
        DebugDraw.NodeBounds(this, Color.BlueViolet);
    }
}