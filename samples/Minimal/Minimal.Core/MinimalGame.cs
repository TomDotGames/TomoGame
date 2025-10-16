using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minimal.Core.Localization;
using TomoGame.Core.SceneGraph;

namespace Minimal.Core
{
    public class MinimalGame : TomoGame.Core.TomoGame
    {
        private MainScene m_scene;
 
        public MinimalGame()
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