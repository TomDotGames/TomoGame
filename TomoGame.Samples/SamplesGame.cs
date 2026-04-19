using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using Color = Microsoft.Xna.Framework.Color;

namespace TomoGame.Samples;

public class SamplesGame : GameBase
{
    public SamplesGame() : base(400, 600)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        SceneRootNode scene = new SceneRootNode(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40);
        SetScene(scene);
        
        TransformNode anotherNode = new TransformNode(new Vector2(10, 20), new Vector2(4, 8), scene);
    }

    protected override void Update(GameTime gameTime)
    {
        DebugDraw.NodeRect(SceneRoot, Color.BlueViolet);
    }
}