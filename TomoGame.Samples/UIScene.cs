using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;
using TomoGame.UI;

namespace TomoGame.Samples;

public class UIScene : SceneRootNode
{
    public UIScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size) : base(graphics, scaleMode, size)
    {
        LayoutNode layout = new LayoutNode("UI/UISceneLayout.xml", this);

        Button spriteSceneButton = layout.FindNode("sprites_button") as Button;
        spriteSceneButton.Clicked += SwitchToSpriteScene;
    }

    private void SwitchToSpriteScene()
    {
        GameBase.Instance.SetScene("sprites");

    }
}
