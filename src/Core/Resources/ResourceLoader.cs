using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TomoGame.Core.Resources
{
    public class ResourceLoader<T>
    {
        private ContentManager m_contentManager;
        private Dictionary<string, T> m_resources = new Dictionary<string, T>();

        public ResourceLoader(IServiceProvider serviceProvider, string strDirectory)
        {
            m_contentManager = new ContentManager(serviceProvider);
            m_contentManager.RootDirectory = "Content";

            // Load all resources in the given directory
            DirectoryInfo dir = new DirectoryInfo(m_contentManager.RootDirectory + "/" + strDirectory);
            Debug.Assert(dir.Exists);

            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string strName = file.Directory.FullName + "/" + Path.GetFileNameWithoutExtension(file.Name);
                T loadedAsset = m_contentManager.Load<T>(strName);
                Debug.Assert(loadedAsset != null);
                m_resources.Add(strName, loadedAsset);
            }
        }

        public T Get(string strName)
        {
            Debug.Assert(m_resources.ContainsKey(strName));
            return m_resources[strName];
        }
    }
}
