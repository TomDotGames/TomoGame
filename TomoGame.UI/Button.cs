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
    private Sprite? _hoveredSprite;
    private Sprite? _downSprite;
    private Sprite? _disabledSprite;
    
    public event Action? Clicked;

    public Button(Node? parent = null) : base(parent)
    {
        _pointable = new NodePointable(this);
        _pointable.Clicked += OnClicked;
    }

    /// <summary>Points the button at a sprite sheet, taking the sprite for each state from it and sizing the
    /// button to the up sprite. Only up is required; a state without a sprite falls back to it.</summary>
    public void SetSprites(string spriteSheetName)
    {
        // these are hardcoded for now but they don't have to be
        _upSprite = ResourceManager.Instance!.GetSprite(spriteSheetName + ".up");
        _hoveredSprite = ResourceManager.Instance!.TryGetSprite(spriteSheetName + ".hovered");
        _downSprite = ResourceManager.Instance!.TryGetSprite(spriteSheetName + ".down");
        _disabledSprite = ResourceManager.Instance!.TryGetSprite(spriteSheetName + ".disabled");

        if (Dbg.Verify(_upSprite))
        {
            IntrinsicSize = _upSprite.SourceRect.Size.ToVector2();
        }
    }

    public override void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? src = element.Attribute("src");
        if (src != null)
        {
            SetSprites(src.Value);
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

        // stretch the sprite onto the button's rect, so sizing the button actually sizes what is drawn. A
        // button left at its sprite's own size renders 1:1.
        Rect worldRect = WorldRect;
        Vector2 sourceSize = spriteToDraw.SourceRect.Size.ToVector2();
        Vector2 renderScale = worldRect.Size / sourceSize;

        spriteBatch.Draw(spriteToDraw.Texture, worldRect.Min, spriteToDraw.SourceRect, Color.White, 0f,
            Vector2.Zero, renderScale, SpriteEffects.None, 0f);
    }

    private void OnClicked(PointerInstance pointerInstance)
    {
        Clicked?.Invoke();
    }
}
