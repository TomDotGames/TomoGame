using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core.Resources;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core.Sprites;

public class SpriteNode : TransformNode
{
    private readonly Sprite _sprite;
    private Rectangle _sourceRect;
    
    public bool FlipX { get; set; }

    public enum AnimationMode
    {
        Loop,
        PingPong,
    };
    public AnimationMode AnimMode { get; set; }
    
    // animation state
    private Sprite.Animation? _animCurrent;
    private float _animCycleStartTime;
    private bool _animPlaybackReversed = false;
    private bool _animStartNew = false;

    public SpriteNode(string spriteName, Vector2 localPosition, Node? parent = null) : base(localPosition, Vector2.Zero, parent)
    {
        _sprite = ResourceManager.Instance!.GetSprite(spriteName);
        _sourceRect = _sprite.SourceRect;
        SetSize(_sprite.SourceRect.Width, _sprite.SourceRect.Height);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);
        SpriteEffects effects = FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(_sprite.Texture, WorldPosition, _sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, effects, 0f);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        if (_animCurrent != null)
        {
            float frameTime = 0.3f; // temp
            float cycleTimeTotal = _animCurrent.Value.FrameCount * frameTime;
            float timeNow = (float)gameTime.TotalGameTime.TotalSeconds;
            if (_animStartNew)
            {
                _animCycleStartTime = timeNow;
                _animStartNew = false;
            }

            float cycleTimeElapsed = timeNow - _animCycleStartTime;
            if (cycleTimeElapsed > cycleTimeTotal)
            {
                float timeSinceCycleEnd = cycleTimeElapsed - cycleTimeTotal;
                _animCycleStartTime = timeNow - timeSinceCycleEnd;
                cycleTimeElapsed = timeSinceCycleEnd;

                if (AnimMode == AnimationMode.PingPong)
                {
                    _animPlaybackReversed = !_animPlaybackReversed;
                }
            }

            int animFrame = (int)(cycleTimeElapsed / frameTime);
            if (_animPlaybackReversed)
            {
                animFrame = _animCurrent.Value.FrameCount - animFrame - 1;
            }
            _sourceRect = _sprite.SourceRect;
            _sourceRect.X = _animCurrent.Value.FirstFrameRect.X + animFrame * (_sourceRect.Width + 1);
        }
    }

    public void PlayAnimation(string animationName)
    {
        _animCurrent = _sprite.GetAnimation(animationName);
        _animStartNew = true;
    }
}