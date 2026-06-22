using Microsoft.Xna.Framework;

namespace TomoGame.Core.Input;

/// <summary>Game component that owns the pointer device and dispatches pointer interactions to registered <see cref="Pointable"/>s.</summary>
public class InputManager : GameComponent
{
    /// <summary>The singleton instance, or null if <see cref="Create"/> has not been called.</summary>
    public static InputManager? Instance { get; private set; }

    private PointerDevice _pointerDevice;
    private List<Pointable> _pointables = new();

    /// <summary>Creates the singleton input manager and adds it to the given game's components.</summary>
    public static InputManager Create(Game game)
    {
        Dbg.Assert(Instance == null);
        InputManager instance = new(game);
        Instance = instance;
        return instance;
    }

    private InputManager(Game game) : base(game)
    {
        game.Components.Add(this);
        _pointerDevice = new MouseDevice();
    }

    /// <summary>Registers a pointable to receive pointer interactions.</summary>
    public void RegisterPointable(Pointable pointable)
    {
        Dbg.Assert(!_pointables.Contains(pointable));
        _pointables.Add(pointable);
    }

    /// <summary>Unregisters a previously registered pointable.</summary>
    public void UnregisterPointable(Pointable pointable)
    {
        Dbg.Assert(_pointables.Contains(pointable));
        _pointables.Remove(pointable);
    }

    /// <summary>Gets the pointer with the given id.</summary>
    public PointerInstance GetPointer(int id)
    {
        return _pointerDevice.GetPointer(id);
    }

    /// <summary>Updates the pointer device and dispatches interactions to registered pointables.</summary>
    public override void Update(GameTime gameTime)
    {
        _pointerDevice.Update(_pointables);
    }
}
