using System;
using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Text;
using TomoGame.UI;

namespace TomoGame.Samples;

/// <summary>The samples front end: one button per sample, each of which swaps the active scene.</summary>
public class MenuScene : SceneRootNode
{
    /// <summary>Every sample the menu can load, with the name shown on its button.</summary>
    public static readonly (string Name, Func<GraphicsDeviceManager, SceneRootNode> Create)[] Samples =
    [
        ("SCENE GRAPH", g => new SceneGraphScene(g, SceneScaleMode.FixedWidth, 40)),
        ("TRANSFORMS", g => new SceneGraphComplexScene(g, SceneScaleMode.FixedWidth, 40)),
        ("SPRITES", g => new SpriteScene(g, SceneScaleMode.FixedWidth, 40)),
        ("XML LAYOUT", g => new UIScene(g, SceneScaleMode.FixedWidth, 40)),
        ("BUTTONS", g => new ButtonScene(g, SceneScaleMode.FixedWidth, 40))
    ];

    private static readonly Vector2 ButtonSize = new(28f, 5f);
    private const float FirstButtonY = 17f;
    private const float ButtonSpacing = 8f;

    public MenuScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        BitmapFont font = ResourceManager.Instance!.GetFont("Fonts/Tiny");

        Label title = new Label("TOMOGAME SAMPLES", font, 2.5f, this);
        title.Anchor = new Vector2(0.5f, 0f);
        title.SetPositionInParentSpace(new Vector2(0.5f, 0f), new Vector2(0f, 6f));

        for (int i = 0; i < Samples.Length; i++)
        {
            // capture the factory itself rather than the loop variable, so every button loads its own sample
            Func<GraphicsDeviceManager, SceneRootNode> create = Samples[i].Create;

            Button button = SampleBrowser.MakeButton(this, font, Samples[i].Name, ButtonSize, 2.5f);
            button.SetPositionInParentSpace(new Vector2(0.5f, 0f), new Vector2(0f, FirstButtonY + (i * ButtonSpacing)));
            button.Clicked += _ => SampleBrowser.ShowSample(create);
        }
    }
}
