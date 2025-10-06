using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.IO;

namespace TomoGame.Core
{
    internal class ResourceManager
    {
        // TODO: figure out how to handle singletons
        private static ResourceManager s_instance;
        public static ResourceManager Instance => s_instance;

        private ContentManager m_contentManager;

        public ResourceManager(IServiceProvider serviceProvider)
        {
            Debug.Assert(s_instance == null);
            s_instance = this;

            Debug.Assert(serviceProvider != null);
            m_contentManager = new ContentManager(serviceProvider);
            m_contentManager.RootDirectory = "Content";
        }

        public void LoadResourcesInDirectory<T>(string sDirectory)
        {
            DirectoryInfo dir = new DirectoryInfo(
                m_contentManager.RootDirectory + "/" + sDirectory
            );
            Debug.Assert(dir.Exists);

            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string sName =
                    file.Directory.FullName + "/" + Path.GetFileNameWithoutExtension(file.Name);
                T loadedAsset = m_contentManager.Load<T>(sName);
                Debug.Assert(loadedAsset != null);
            }

            Log($"Loaded {files.Length} file(s) in {sDirectory}");
        }

        public T GetResource<T>(string sName)
        {
            return m_contentManager.Load<T>(sName);
        }

        private static void Log(string sMessage)
        {
            // TODO: good logging
            Debug.WriteLine("[ResourceManager] " + sMessage);
        }
    }
}
