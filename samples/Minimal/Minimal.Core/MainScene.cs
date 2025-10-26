using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace Minimal.Core
{
    internal class MainScene : TomoGame.Core.SceneGraph.Scene
    {
        public MainScene(GraphicsDeviceManager graphics, ESceneScaleMode eScaleMode, int nSize) : base(graphics, eScaleMode, nSize)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            ResourceLoader<Texture2D> spriteLoader = new ResourceLoader<Texture2D>("sprites");
            CompositeNode spriteNode = new CompositeNode(RootNode);
            SpriteRenderer renderer = new SpriteRenderer(spriteNode, spriteLoader.Get("bear"));
        }
    }
}
