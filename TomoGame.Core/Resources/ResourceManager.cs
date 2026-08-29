using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Sprites;
using TomoGame.Core.Text;

namespace TomoGame.Core.Resources;

/// <summary>Manages loading and retrieval of game assets.</summary>
public class ResourceManager
{
    /// <summary>The global resource manager instance.</summary>
    public static ResourceManager? Instance { get; private set; }

    private ContentManager _contentManager;
    private HashSet<string> _loadedResources = [];
    private SpriteRegistry _spriteRegistry;
    private Dictionary<string, BitmapFont> _fonts = [];

    /// <summary>Creates a new ResourceManager. Only one instance may exist at a time.</summary>
    internal ResourceManager(IServiceProvider serviceProvider)
    {
        Dbg.Assert(Instance == null);
        Instance = this;

        _contentManager = new ContentManager(serviceProvider);
        _contentManager.RootDirectory = "Content";
        
        _spriteRegistry = new SpriteRegistry();
    }

    /// <summary>Loads all assets of type <typeparamref name="T"/> from a content subdirectory.</summary>
    public void LoadResourcesInDirectory<T>(string directory)
    {
        DirectoryInfo dir = new DirectoryInfo(
            _contentManager.RootDirectory + "/" + directory
        );
        Dbg.Assert(dir.Exists);

        FileInfo[] files = dir.GetFiles("*.xnb", SearchOption.AllDirectories);
        string rootPath = Path.GetFullPath(_contentManager.RootDirectory);
        foreach (FileInfo file in files)
        {
            string assetPath = Path.Combine(file.Directory!.FullName, Path.GetFileNameWithoutExtension(file.Name));
            string name = Path.GetRelativePath(rootPath, assetPath).Replace('\\', '/');
            if (!Dbg.Verify(!_loadedResources.Contains(name)))
                continue;

            T resource = _contentManager.Load<T>(name);
            _loadedResources.Add(name);
            
            // sprites
            Texture2D? texture = resource as Texture2D;
            if (texture != null)
            {
                _spriteRegistry.LoadSpriteSheet(name, texture);
            }
        }

        Log.Info($"Loaded {files.Length} file(s) in {directory}");
    }

    /// <summary>Returns a previously loaded asset by name. Asserts if the asset was not loaded.</summary>
    public T? GetResource<T>(string name)
    {
        Dbg.Assert(_loadedResources.Contains(name));
        return _contentManager.Load<T>(name);
    }

    /// <summary>Loads a bitmap font sheet and registers it under its asset name. The grid metrics are passed
    /// in rather than described alongside the texture; a descriptor file would be the next step once there is
    /// more than a font or two.</summary>
    public BitmapFont LoadFont(string name, int cellWidth, int cellHeight, char firstChar, int columns)
    {
        Texture2D texture = _contentManager.Load<Texture2D>(name);
        BitmapFont font = new BitmapFont(texture, cellWidth, cellHeight, firstChar, columns);

        Dbg.Verify(!_fonts.ContainsKey(name), $"font '{name}' is already loaded");
        _fonts[name] = font;

        Log.Info($"Loaded font {name}");
        return font;
    }

    /// <summary>Returns a previously loaded font by name. Asserts if it was not loaded.</summary>
    public BitmapFont GetFont(string name)
    {
        Dbg.Assert(_fonts.ContainsKey(name));
        return _fonts[name];
    }

    /// <summary>Returns a sprite by name from the sprite registry. Asserts if not found.</summary>
    public Sprite GetSprite(string name)
    {
        return _spriteRegistry.GetSprite(name);
    }
}
