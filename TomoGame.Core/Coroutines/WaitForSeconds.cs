namespace TomoGame.Core.Coroutines;

/// <summary>Yield one of these to pause a coroutine for a while.</summary>
public class WaitForSeconds : Coroutine
{
    private readonly float _duration;
    private float _remaining;

    public WaitForSeconds(float duration)
    {
        _duration = duration;
    }

    public override void Start()
    {
        _remaining = _duration;
    }

    public override void Reset()
    {
        base.Reset();
        _remaining = _duration;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        _remaining -= deltaTime;
        if (_remaining <= 0f)
        {
            SetFinished();
        }
    }

    public override void FinishImmediately()
    {
        SetFinished();
    }
}
