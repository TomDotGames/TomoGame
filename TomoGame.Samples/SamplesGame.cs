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

        // 16 columns of 8x11 glyphs, running from space through to '~'
        ResourceManager.Instance.LoadFont("Fonts/Tiny", 8, 11, ' ', 16);
    }

    protected override void Initialize()
    {
        base.Initialize();

        AddScene(SampleBrowser.Menu, new MenuScene(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40));
        AddSample(SampleBrowser.SceneGraphScene, new SceneGraphScene(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40));
        AddSample(SampleBrowser.SpriteScene, new SpriteScene(Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40));

        SetScene(SampleBrowser.Menu);
    }

    /// <summary>Registers a sample and gives it a way back to the menu.</summary>
    private void AddSample(string name, SceneRootNode scene)
    {
        SampleBrowser.AddBackButton(scene);
        AddScene(name, scene);
    }
 }