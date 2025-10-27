using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites
{
    public class SpriteNode : CompositeNode
    {
        public SpriteNode(Node parent, Texture2D texture) : base(parent)
        {
            SpriteRenderer renderer = new SpriteRenderer(this, texture);
        }
    }
}
