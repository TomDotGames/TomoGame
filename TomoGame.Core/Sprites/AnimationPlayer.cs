namespace TomoGame.Core.Sprites;

/// <summary>Drives frame-based sprite animation. Call <see cref="Update"/> each tick and read <see cref="CurrentFrame"/> to get the current frame index.</summary>
public class AnimationPlayer
{
    /// <summary>The currently playing animation, or null if none is playing.</summary>
    public Sprite.Animation? Animation { get; private set; }

    /// <summary>The current frame index within the playing animation.</summary>
    public int CurrentFrame { get; private set; }

    /// <summary>Controls how the animation loops.</summary>
    public enum AnimationMode
    {
        /// <summary>Plays once and stops on the last frame.</summary>
        OneShot,
        /// <summary>Loops from the beginning when the animation ends.</summary>
        Loop,
        /// <summary>Reverses direction each time the animation ends.</summary>
        PingPong,
    };
    private AnimationMode _mode;

    private bool _playbackReversed = false;
    private float _cycleStartTime;

    /// <summary>Starts playing the given animation in the specified mode.</summary>
    public void PlayAnimation(Sprite.Animation animation, AnimationMode mode)
    {
        Animation = animation;
        _cycleStartTime = Time.TotalSeconds;
        _mode = mode;
        _playbackReversed = false;
    }

    /// <summary>Advances the animation state. Call once per game tick.</summary>
    public void Update()
    {
        if (!Animation.HasValue)
            return;

        float cycleDuration = Animation.Value.FrameCount * Animation.Value.FrameTime;
        float timeSinceCycleStart = Time.TotalSeconds - _cycleStartTime;
        if (timeSinceCycleStart > cycleDuration)
        {
            float timeSinceCycleEnd = timeSinceCycleStart % cycleDuration;
            switch (_mode)
            {
                case AnimationMode.OneShot:
                    Animation = null;
                    return;
                case AnimationMode.Loop:
                    _cycleStartTime = Time.TotalSeconds - timeSinceCycleEnd;
                    break;
                case AnimationMode.PingPong:
                    _cycleStartTime = Time.TotalSeconds - timeSinceCycleEnd;
                    _playbackReversed = !_playbackReversed;
                    // skip to the next frame so that end frames don't have double duration
                    _cycleStartTime -= Animation.Value.FrameTime;
                    timeSinceCycleStart += Animation.Value.FrameTime;
                    break;
            }
        }

        int frameNum = (int)(timeSinceCycleStart / Animation.Value.FrameTime);
        frameNum %= Animation.Value.FrameCount;
        if (_playbackReversed)
        {
            frameNum = Animation.Value.FrameCount - frameNum - 1;
        }
        CurrentFrame = frameNum;
    }
}
