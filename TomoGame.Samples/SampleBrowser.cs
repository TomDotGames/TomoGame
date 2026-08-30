using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.UI;

namespace TomoGame.Samples;

/// <summary>Names the scenes the browser switches between, and puts a way back to the menu on each sample.</summary>
public static class SampleBrowser
{
    public const string Menu = "menu";
    public const string SceneGraphScene = "scenegraph";
    public const string SpriteScene = "sprites";

    private const float BackButtonZOrder = 2000f;
    private static readonly Vector2 BackButtonSize = new(14f, 5f);

    /// <summary>Overlays a button that returns to the menu, so the samples themselves do not have to know
    /// anything about the browser.</summary>
    public static void AddBackButton(SceneRootNode scene)
    {
        Button back = new Button(scene);
        back.SetSprites("Sprites/Button");
        back.IntrinsicSize = BackButtonSize;
        back.OriginUV = Vector2.Zero;
        back.LocalPosition = new Vector2(1f, 1f);

        // above even the debug overlay: a sample that covers this leaves no way back to the menu
        back.ZOrder = BackButtonZOrder;
        back.Clicked += () => GameBase.Instance!.SetScene(Menu);

        Label label = new Label("MENU", ResourceManager.Instance!.DefaultFont!, 2.5f, back);
        label.OriginUV = new Vector2(0.5f, 0.5f);
        label.LocalPosition = back.UVToChildSpace(new Vector2(0.5f, 0.5f));
    }
}
