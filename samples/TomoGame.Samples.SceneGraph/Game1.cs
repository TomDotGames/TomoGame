using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minimal.Core;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Samples.SceneGraph
{
    public class Game1 : TomoGame.Core.TomoGame
    {
        private MainScene m_scene;

        public Game1()
        {

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_scene = new MainScene(Graphics, Scene.ESceneScaleMode.FixedHeight, 200);
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            SetScene(m_scene);
        }
    }
}
