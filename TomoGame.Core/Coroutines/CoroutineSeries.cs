using System.Collections;

namespace TomoGame.Core.Coroutines;

/// <summary>Runs coroutines one after another.</summary>
public class CoroutineSeries : Coroutine
{
    private readonly List<Coroutine> _coroutines;
    private int _currentIndex;

    public CoroutineSeries(params Coroutine[] coroutines)
    {
        _coroutines = coroutines.ToList();
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

        // the original indexed straight into the list, so an empty series threw
        while (_currentIndex >= _coroutines.Count || _coroutines[_currentIndex].IsFinished)
        {
            ++_currentIndex;
            if (_currentIndex >= _coroutines.Count)
            {
                SetFinished();
                return;
            }
        }

        _coroutines[_currentIndex].Update(deltaTime);
    }

    public override void FinishImmediately()
    {
        for (int i = _currentIndex; i < _coroutines.Count; ++i)
        {
            _coroutines[i].FinishImmediately();
        }

        SetFinished();
    }
}
