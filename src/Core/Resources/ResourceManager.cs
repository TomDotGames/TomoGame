using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.IO;

namespace TomoGame.Core.Resources
{
    public class ResourceManager
    {
        private readonly ContentManager _contentManager;

        public ResourceManager( IServiceProvider serviceProvider )
        {
            Debug.Assert( serviceProvider != null );
            _contentManager = new ContentManager( serviceProvider );
            _contentManager.RootDirectory = "Content";
        }

        public void LoadResourcesInDirectory<T>( string sDirectory )
        {
            DirectoryInfo dir = new DirectoryInfo( _contentManager.RootDirectory + "/" + sDirectory );
            Debug.Assert( dir.Exists );

            FileInfo[] files = dir.GetFiles( "*.*", SearchOption.AllDirectories );
            foreach ( FileInfo file in files )
            {
                string sPath = Path.GetRelativePath( _contentManager.RootDirectory, file.FullName );
                sPath = Path.ChangeExtension( sPath, null );
                T loadedAsset = _contentManager.Load<T>( sPath );
                Debug.Assert( loadedAsset != null );
            }

            Log( $"Loaded {files.Length} file(s) in {sDirectory}" );
        }

        public T GetResource<T>( string sName )
        {
            return _contentManager.Load<T>( sName );
        }

        private static void Log( string sMessage )
        {
            Debug.WriteLine( "[ResourceManager] " + sMessage );
        }
    }
}