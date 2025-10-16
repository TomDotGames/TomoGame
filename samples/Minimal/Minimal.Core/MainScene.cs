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
            SpriteRenderer renderer = new SpriteRenderer(spriteLoader.Get("bear"));
            Node spriteNode = new Node(RootNode);
            spriteNode.AddComponent(renderer);
        }
    }
}
