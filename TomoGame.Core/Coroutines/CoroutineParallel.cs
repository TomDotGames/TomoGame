using System.Collections;

namespace TomoGame.Core.Coroutines;

/// <summary>Runs coroutines side by side, finishing once all of them have. Yield one to wait for the lot, eg:
/// <code>yield return new CoroutineParallel(a.Animate(), b.Animate());</code></summary>
public class CoroutineParallel : Coroutine
{
    private readonly List<Coroutine> _coroutines;

    // without this, an empty `new CoroutineParallel()` would be ambiguous between the two params overloads
    public CoroutineParallel()
    {
        _coroutines = [];
    }

    public CoroutineParallel(params Coroutine[] coroutines)
    {
        _coroutines = coroutines.ToList();
    }

    public CoroutineParallel(params IEnumerator[] enumerators)
    {
        _coroutines = enumerators.Select(enumerator => (Coroutine)new WrappedEnumerator(enumerator)).ToList();
    }

    public void Add(Coroutine coroutine)
    {
        _coroutines.Add(coroutine);
    }

    public void Add(IEnumerator enumerator)
    {
        _coroutines.Add(new WrappedEnumerator(enumerator));
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        bool allFinished = true;
        foreach (Coroutine coroutine in _coroutines)
        {
            if (coroutine.IsFinished)
                continue;

            coroutine.Update(deltaTime);
            allFinished &= coroutine.IsFinished;
        }

        if (allFinished)
        {
            SetFinished();
        }
    }

    public override void FinishImmediately()
    {
        foreach (Coroutine coroutine in _coroutines)
        {
            if (!coroutine.IsFinished)
                coroutine.FinishImmediately();
        }

        SetFinished();
    }
}
