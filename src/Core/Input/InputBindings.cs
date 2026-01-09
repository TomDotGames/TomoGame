using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TomoGame.Core.Input;

public class InputBindings
{
    private class BindingsConfig
    {
        public Dictionary<string, string> keys { get; set; }
    }

    private Dictionary<Keys, string> _keyBindings = new Dictionary<Keys, string>();

    public InputBindings()
    {
        LoadBindings();
    }

    private void LoadBindings()
    {
        const string jsonPath = "Content/scripts/input_bindings.json";
        using Stream stream = TitleContainer.OpenStream(jsonPath); 
        using StreamReader reader = new StreamReader( stream );
        string json = reader.ReadToEnd();
        BindingsConfig config = JsonSerializer.Deserialize<BindingsConfig>(json);
        foreach (KeyValuePair<string, string> kvp in config.keys)
        {
            if (Enum.TryParse<Keys>(kvp.Value, true, out Keys key))
            {
                _keyBindings.Add(key, kvp.Key);
            }
        }
    }
}