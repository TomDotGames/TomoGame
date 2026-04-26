using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Sprites;

namespace TomoGame.Core.Resources;

public class ResourceManager
{
    public static ResourceManager? Instance { get; private set; }

    private ContentManager _contentManager;
    private HashSet<string> _loadedResources = [];
    private SpriteRegistry _spriteRegistry;

    public ResourceManager(IServiceProvider serviceProvider)
    {
        Dbg.Assert(Instance == null);
        Instance = this;

        _contentManager = new ContentManager(serviceProvider);
        _contentManager.RootDirectory = "Content";
        
        _spriteRegistry = new SpriteRegistry();
    }

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

    public T? GetResource<T>(string name)
    {
        Dbg.Assert(_loadedResources.Contains(name));
        return _contentManager.Load<T>(name);
    }

    public Sprite GetSprite(string name)
    {
        return _spriteRegistry.GetSprite(name);
    }
}
