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
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        ResourceManager.Instance.LoadResourcesInDirectory<Texture2D>("Sprites");
    }

    protected override void Initialize()
    {
        base.Initialize();

        AddScene("ui", new UIScene(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40));
        AddScene("sprites", new SpriteScene(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40));
        SetScene("ui");
    }
 }