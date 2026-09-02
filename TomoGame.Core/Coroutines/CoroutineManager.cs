using System.Collections;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.Coroutines;

/// <summary>Game component that advances every running coroutine once per frame.</summary>
public class CoroutineManager : GameComponent
{
    /// <summary>The global coroutine manager instance.</summary>
    public static CoroutineManager? Instance { get; private set; }

    private readonly List<Coroutine> _coroutines = new();

    internal CoroutineManager(Game game) : base(game)
    {
        Dbg.Assert(Instance == null);
        Instance = this;

        game.Components.Add(this);
    }

    public static Coroutine? Run(IEnumerator enumerator)
    {
        return Run(new WrappedEnumerator(enumerator));
    }

    public static Coroutine? Run(Coroutine coroutine)
    {
        if (!Dbg.Verify(Instance != null))
            return null;

        Instance._coroutines.Add(coroutine);
        return coroutine;
    }

    public override void Update(GameTime gameTime)
    {
        if (_coroutines.Count == 0)
            return;

        // copy, because a coroutine can start another one
        foreach (Coroutine coroutine in _coroutines.ToList())
        {
            coroutine.Update(Time.TickSeconds);
        }

        _coroutines.RemoveAll(coroutine => coroutine.IsFinished);
    }

    /// <summary>Runs everything to its end state at once, including anything spawned along the way.</summary>
    public void FinishAllImmediately()
    {
        while (_coroutines.Count > 0)
        {
            foreach (Coroutine coroutine in _coroutines.ToList())
            {
                coroutine.FinishImmediately();
            }

            _coroutines.RemoveAll(coroutine => coroutine.IsFinished);
        }
    }
}
