using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomoGame.Core.Sprites;

/// <summary>Stores and provides access to loaded sprites, populated from sprite sheet textures and optional JSON atlas descriptors.</summary>
public class SpriteRegistry
{
    private class SpriteData
    {
        public int[]? Rect { get; set; }

        public class AnimationData
        {
            public int Frames { get; set; }
        }
        public Dictionary<string, AnimationData>? Animations { get; set; }
    }
    
    private Dictionary<string, Sprite> _sprites = new();
    
    /// <summary>Loads a sprite sheet texture and its optional JSON atlas descriptor. Sprites are registered as <c>sheetName.spriteName</c>.</summary>
    public void LoadSpriteSheet(string name, Texture2D sheetTexture)
    {
        // base sprite for the whole sheet
        Sprite baseSprite = new Sprite(sheetTexture);
        _sprites.Add(name.ToLower(), baseSprite);
        
        // if there is a json descriptor then load and handle that
        string jsonPath = $"Content/{name}.json";
        if (File.Exists(jsonPath))
        {
            Stream stream = TitleContainer.OpenStream(jsonPath);
            StreamReader reader = new StreamReader(stream);
            string strJson = reader.ReadToEnd();
            reader.Close();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Dictionary<string, SpriteData>? sprites = JsonSerializer.Deserialize<Dictionary<string, SpriteData>>(strJson, options);
            foreach (KeyValuePair<string, SpriteData> sprite in sprites ?? [])
            {
                LoadSpriteFromData(name, sprite.Key, sheetTexture, sprite.Value);
            }
        }
    }

    private void LoadSpriteFromData(string sheetName, string spriteName, Texture2D sheetTexture, SpriteData spriteData)
    {
        if (!Dbg.Verify(spriteData.Rect != null)) return;
        Rectangle sourceRect = new Rectangle(spriteData.Rect![0], spriteData.Rect[1], spriteData.Rect[2], spriteData.Rect[3]);
        Dictionary<string, Sprite.Animation> animations = [];
        if (spriteData.Animations != null)
        {
            foreach (KeyValuePair<string, SpriteData.AnimationData> animData in spriteData.Animations)
            {
                Sprite.Animation animation = new Sprite.Animation();
                animation.FirstFrameRect = sourceRect; // todo: offsets
                animation.FrameCount = animData.Value.Frames;
                Dbg.Assert(!animations.ContainsKey(animData.Key));
                animations.Add(animData.Key, animation);
            }
        }

        Sprite sprite = new Sprite(sheetTexture, sourceRect, animations);
        string name = $"{sheetName}.{spriteName}";
        _sprites.Add(name.ToLower(), sprite);
    }

    /// <summary>Returns a sprite by name. Name is case-insensitive. Asserts if not found.</summary>
    public Sprite GetSprite(string name)
    {
        name = name.ToLower();
        Dbg.Assert(_sprites.ContainsKey(name));
        return _sprites[name];
    }
}