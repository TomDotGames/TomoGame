using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.Samples;

public class UIScene : SceneRootNode
{
    private SpriteNode _dog;
    private bool _dogGoingRight = true;
    
    public UIScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size) : base(graphics, scaleMode, size)
    {
        LayoutNode layout = new LayoutNode("UI/UISceneLayout.xml", this);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);
        
        DebugDraw.NodeRect(this, Color.White);
    }
}