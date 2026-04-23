using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.Sprites;

public class SpriteRegistry
{
    private Dictionary<string, Sprite> _sprites = new();
    
    public void LoadSpriteSheet(string name, Texture2D sheetTexture)
    {
        // base sprite for the whole sheet
        Sprite baseSprite = new Sprite(sheetTexture);
        _sprites.Add(name, baseSprite);
    }

    public Sprite GetSprite(string name)
    {
        Dbg.Assert(_sprites.ContainsKey(name));
        return _sprites[name];
    }
}