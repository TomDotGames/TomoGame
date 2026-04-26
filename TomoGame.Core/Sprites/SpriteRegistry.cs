using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.Sprites;

public class SpriteRegistry
{
    private class SpriteData
    {
        public int[] Rect { get; set; }

        public class AnimationData
        {
            public int Frames { get; set; }
        }
        public Dictionary<string, AnimationData>? Animations { get; set; }
    }
    
    private Dictionary<string, Sprite> _sprites = new();
    
    public void LoadSpriteSheet(string name, Texture2D sheetTexture)
    {
        // base sprite for the whole sheet
        Sprite baseSprite = new Sprite(sheetTexture);
        _sprites.Add(name.ToLower(), baseSprite);
        
        // if there is a json descriptor then load and handle that
        Stream stream = TitleContainer.OpenStream($"Content/{name}.json");
        StreamReader reader = new StreamReader(stream);
        string strJson = reader.ReadToEnd();
        reader.Close();

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        Dictionary<string, SpriteData> sprites = JsonSerializer.Deserialize<Dictionary<string, SpriteData>>(strJson, options);
        foreach (KeyValuePair<string, SpriteData> sprite in sprites)
        {
            LoadSpriteFromData(name, sprite.Key, sheetTexture, sprite.Value);
        }
    }

    private void LoadSpriteFromData(string sheetName, string spriteName, Texture2D sheetTexture, SpriteData spriteData)
    {
        Rectangle sourceRect = new Rectangle(spriteData.Rect[0], spriteData.Rect[1], spriteData.Rect[2], spriteData.Rect[3]);
        List<Sprite.Animation> animations = [];
        if (spriteData.Animations != null)
        {
            foreach (KeyValuePair<string, SpriteData.AnimationData> animData in spriteData.Animations)
            {
                Sprite.Animation animation = new Sprite.Animation();
                animation.FirstFrameRect = sourceRect; // todo: offsets
                animation.FrameCount = animData.Value.Frames;
                animations.Add(animation);
            }
        }

        Sprite sprite = new Sprite(sheetTexture, sourceRect, animations.ToArray());
        string name = $"{sheetName}.{spriteName}";
        _sprites.Add(name.ToLower(), sprite);
    }

    public Sprite GetSprite(string name)
    {
        name = name.ToLower();
        Dbg.Assert(_sprites.ContainsKey(name));
        return _sprites[name];
    }
}