using System.Collections;

namespace TomoGame.Core.Coroutines;

/// <summary>Runs an <see cref="IEnumerator"/> as a coroutine. Yielding a coroutine or another enumerator from
/// it waits for that to finish before stepping on.</summary>
public class WrappedEnumerator : Coroutine
{
    public override object? Current => _enumerator.Current;

    private readonly IEnumerator _enumerator;
    private WrappedEnumerator? _nested;

    public WrappedEnumerator(IEnumerator enumerator)
    {
        _enumerator = enumerator;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        bool advance = true;
        if (Current is Coroutine coroutine && !coroutine.IsFinished)
        {
            coroutine.Update(deltaTime);
            advance = coroutine.IsFinished;
        }
        else if (Current is IEnumerator nested)
        {
            // the original built a fresh wrapper every frame, so a nested enumerator was restarted rather
            // than resumed. It happened to work because the enumerator itself holds the state.
            _nested ??= new WrappedEnumerator(nested);
            _nested.Update(deltaTime);
            advance = _nested.IsFinished;
        }

        if (advance)
        {
            _nested = null;
            if (!_enumerator.MoveNext())
            {
                SetFinished();
            }
        }
    }

    public override void FinishImmediately()
    {
        bool moved = true;
        while (moved)
        {
            if (Current is Coroutine coroutine)
            {
                coroutine.FinishImmediately();
            }
            else if (Current is IEnumerator nested)
            {
                new WrappedEnumerator(nested).FinishImmediately();
            }

            moved = _enumerator.MoveNext();
        }

        _nested = null;
        SetFinished();
    }
}
