using TomoGame.Core.Coroutines;

namespace TomoGame.Core.Tweening;

/// <summary>Eases a value from wherever it is to a target over a duration. Reads through a getter and writes
/// through a setter, so it can drive anything without owning it.</summary>
public class Tween<T> : Coroutine
{
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;
    private readonly Func<T, T, float, T> _ease;
    private readonly T _endValue;
    private readonly float _duration;

    private T _startValue = default!;
    private float _elapsed;

    public Tween(Func<T> getter, Action<T> setter, T target, float duration, Func<T, T, float, T> ease)
    {
        _getter = getter;
        _setter = setter;
        _endValue = target;
        _duration = duration;
        _ease = ease;
    }

    /// <summary>The start value is read here rather than in the constructor, so a queued tween eases from
    /// where the value has got to by the time it runs.</summary>
    public override void Start()
    {
        _startValue = _getter();
        _elapsed = 0f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        _elapsed += deltaTime;

        float t = _duration == 0f ? 1f : _elapsed / _duration;
        if (t >= 1f)
        {
            // the original only finished past 1, so a tween always ran a frame long
            t = 1f;
            SetFinished();
        }

        _setter(_ease(_startValue, _endValue, t));
    }

    public override void FinishImmediately()
    {
        _setter(_endValue);
        SetFinished();
    }
}
