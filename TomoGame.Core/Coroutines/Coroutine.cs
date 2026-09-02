namespace TomoGame.Core.Coroutines;

/// <summary>A unit of work advanced once per frame. Yield one from an <see cref="System.Collections.IEnumerator"/>
/// to wait for it to finish.</summary>
public abstract class Coroutine
{
    /// <summary>What this coroutine is currently waiting on, if anything.</summary>
    public virtual object? Current => null;

    public bool IsFinished { get; private set; }
    private bool _running;

    /// <summary>Called on the first update, so a coroutine reads the world as it is when it starts running
    /// rather than when it was created.</summary>
    public virtual void Start() { }

    public virtual void Update(float deltaTime)
    {
        if (!_running)
        {
            Start();
            _running = true;
        }
    }

    /// <summary>Jumps to the end state, for when the outcome matters but the animation doesn't.</summary>
    public abstract void FinishImmediately();

    public virtual void Reset()
    {
        IsFinished = false;
    }

    protected void SetFinished()
    {
        IsFinished = true;
        _running = false;
    }
}
