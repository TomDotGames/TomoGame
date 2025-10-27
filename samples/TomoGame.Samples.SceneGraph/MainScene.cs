
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace Minimal.Core
{
    internal class MainScene : TomoGame.Core.SceneGraph.Scene
    {
        private SpriteNode m_spriteBear;

        public MainScene(GraphicsDeviceManager graphics, ESceneScaleMode eScaleMode, int nSize) : base(graphics, eScaleMode, nSize)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            ResourceLoader<Texture2D> spriteLoader = new ResourceLoader<Texture2D>("sprites");
            m_spriteBear = new SpriteNode(RootNode, spriteLoader.Get("bear"));
            m_spriteBear.SetPositionLocal(new Vector2(0, 0), Rect.UVTopLeft);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float flDt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 vVelocity = new Vector2(10f * flDt, 4f * flDt);
            m_spriteBear.Translate(vVelocity);
        }
    }
}