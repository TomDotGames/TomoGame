using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.Samples;

public class SpriteScene : SceneRootNode
{
    private SpriteNode _dog;
    private bool _dogGoingRight = true;
    
    public SpriteScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size) : base(graphics, scaleMode, size)
    {
        SpriteNode house = new SpriteNode("Sprites/Samples.House", new Vector2(15, 20), this);
        SpriteNode car = new SpriteNode("Sprites/Samples.Car", new Vector2(25, 35), this);
        SpriteNode flower = new SpriteNode("Sprites/Samples.Flower", new Vector2(5, 10), this);
        flower.PlayAnimation("wave");
        flower.AnimMode = SpriteNode.AnimationMode.PingPong;
        SpriteNode flower2 = new SpriteNode("Sprites/Samples.Flower", new Vector2(28, 7), this);
        flower2.AnimMode = SpriteNode.AnimationMode.PingPong;
        flower2.PlayAnimation("wave");
        _dog = new SpriteNode("Sprites/Samples.Dog", new Vector2(10, 50), this);
        _dog.PlayAnimation("wag");
    }
    
    protected override void OnUpdate(GameTime gameTime)
    {
        // move the dog
        float moveSpeed = 10f;
        float moveAmount = (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
        if (_dogGoingRight)
        {
            _dog.FlipX = false;
            _dog.Translate(new Vector2(moveAmount, 0));
            if (_dog.WorldPosition.X > 30)
            {
                _dogGoingRight = false;
            }
        }
        else
        {
            _dog.FlipX = true;
            _dog.Translate(new Vector2(-moveAmount, 0));
            if (_dog.WorldPosition.X < 5)
            {
                _dogGoingRight = true;
            } 
        }
    }
}