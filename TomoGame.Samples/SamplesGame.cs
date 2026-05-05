using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using Color = Microsoft.Xna.Framework.Color;

namespace TomoGame.Samples;

public class SamplesGame : GameBase
{
    public SamplesGame() : base(400, 600)
    {
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        ResourceManager.Instance.LoadResourcesInDirectory<Texture2D>("Sprites");
    }

    protected override void Initialize()
    {
        base.Initialize();

        SceneRootNode scene = new UIScene(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40);
        SetScene(scene);
    }
}
