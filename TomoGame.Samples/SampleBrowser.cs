using System;
using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Text;
using TomoGame.UI;

namespace TomoGame.Samples;

/// <summary>Moves between the menu and the samples, and builds the buttons both of them use.</summary>
public static class SampleBrowser
{
    private static readonly Vector2 BackButtonSize = new(11f, 4f);

    /// <summary>Shows the sample menu.</summary>
    public static void ShowMenu()
    {
        GameBase game = GameBase.Instance!;
        game.SetScene(new MenuScene(game.Graphics, SceneRootNode.SceneScaleMode.FixedWidth, 40));
    }

    /// <summary>Builds a sample's scene and shows it, with a button to get back to the menu.</summary>
    public static void ShowSample(Func<GraphicsDeviceManager, SceneRootNode> create)
    {
        GameBase game = GameBase.Instance!;
        SceneRootNode scene = create(game.Graphics);

        // added before the scene is handed over, so it is initialised along with the rest of the graph
        BitmapFont font = ResourceManager.Instance!.GetFont("Fonts/Tiny");
        Button back = MakeButton(scene, font, "MENU", BackButtonSize, 2f);
        back.Anchor = Vector2.Zero; // pin its top-left corner, so the inset reads as a true margin
        back.SetPositionInParentSpace(new Vector2(0f, 0f), new Vector2(1f, 1f));
        back.Clicked += _ => ShowMenu();

        game.SetScene(scene);
    }

    /// <summary>A button of a given size with a caption centred on it.</summary>
    public static Button MakeButton(Node parent, BitmapFont font, string caption, Vector2 size, float textHeight)
    {
        Button button = new Button("Sprites/Button", parent);
        button.Anchor = new Vector2(0.5f, 0.5f);
        button.SetIntrinsicSize(size);

        Label label = new Label(caption, font, textHeight, button);
        label.Anchor = new Vector2(0.5f, 0.5f);
        label.SetPositionInParentSpace(new Vector2(0.5f, 0.5f), Vector2.Zero);

        return button;
    }
}
