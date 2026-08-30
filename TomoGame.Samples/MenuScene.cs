using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.UI;

namespace TomoGame.Samples;

/// <summary>The samples front end: one button per sample, each of which switches the active scene.</summary>
public class MenuScene : SceneRootNode
{
    public MenuScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        LayoutNode layout = new LayoutNode("UI/MenuLayout.xml", this);

        Wire(layout, "scenegraph_button", SampleBrowser.SceneGraphScene);
        Wire(layout, "sprites_button", SampleBrowser.SpriteScene);
    }

    private static void Wire(LayoutNode layout, string buttonName, string sceneName)
    {
        if (layout.FindNode(buttonName) is not Button button)
        {
            Dbg.Verify(false, $"menu layout has no button named '{buttonName}'");
            return;
        }

        button.Clicked += () => GameBase.Instance!.SetScene(sceneName);
    }
}
