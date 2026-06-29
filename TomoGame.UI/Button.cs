using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.Input;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.UI;

[LayoutNode("Button")]
public class Button : Node
{
    private readonly NodePointable _pointable;

    private Sprite _upSprite;
    private Sprite _hoveredSprite;
    private Sprite _downSprite;
    private Sprite _disabledSprite;
    
    public event Action? Clicked;

    public Button(Node? parent = null) : base(parent)
    {
        _pointable = new NodePointable(this);
        _pointable.Clicked += OnClicked;
    }

    public override void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? src = element.Attribute("src");
        if (src != null)
        {
            // these are hardcoded for now but they don't have to be
            string upSpriteName = src.Value + ".up";
            _upSprite = ResourceManager.Instance!.GetSprite(upSpriteName);

            string hoveredSpriteName = src.Value + ".hovered";
            _hoveredSprite = ResourceManager.Instance!.TryGetSprite(hoveredSpriteName);

            string downSpriteName = src.Value + ".down";
            _downSprite = ResourceManager.Instance!.TryGetSprite(downSpriteName);

            string disabledSpriteName = src.Value + ".disabled";
            _disabledSprite = ResourceManager.Instance!.TryGetSprite(disabledSpriteName);
        }

        if (Dbg.Verify(_upSprite))
        {
            IntrinsicSize = _upSprite.SourceRect.Size.ToVector2();
        }

        base.ApplyLayoutAttributes(element);
    }
    
    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);

        Sprite spriteToDraw = _upSprite;
        if (_pointable.IsAnyPointerSelecting)
        {
            spriteToDraw = _downSprite ?? spriteToDraw;
        }
        else if (_pointable.IsAnyPointerInside)
        {
            spriteToDraw = _hoveredSprite ?? spriteToDraw;
        }

        if (!Dbg.Verify(spriteToDraw))
            return;
        
        spriteBatch.Draw(spriteToDraw.Texture, WorldRect.Min, spriteToDraw.SourceRect, Color.White);
    }

    private void OnClicked(PointerInstance pointerInstance)
    {
        Clicked?.Invoke();
    }
}
