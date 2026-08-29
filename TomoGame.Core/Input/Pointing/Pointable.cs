using Microsoft.Xna.Framework;

namespace TomoGame.Core.Input;

/// <summary>Base class for objects that can receive pointer interactions (enter/exit/select/click).</summary>
public abstract class Pointable
{
    /// <summary>Raised when a pointer enters this pointable.</summary>
    public event Action<PointerInstance>? Entered;
    /// <summary>Raised when a pointer exits this pointable.</summary>
    public event Action<PointerInstance>? Exited;
    /// <summary>Raised when a pointer begins selecting this pointable.</summary>
    public event Action<PointerInstance>? Selected;
    /// <summary>Raised when a pointer stops selecting this pointable.</summary>
    public event Action<PointerInstance>? Unselected;
    /// <summary>Raised when a pointer is released while still inside this pointable.</summary>
    public event Action<PointerInstance>? Clicked;

    /// <summary>When true, pointer events are not passed down to pointables underneath this one.</summary>
    public bool IsExclusive => _exclusive;
    private bool _exclusive = true;

    /// <summary>When exclusive, the pointable with the highest priority is chosen for interaction.</summary>
    public virtual float Priority => 0f;

    private List<PointerInstance> _pointersInside = new();
    /// <summary>True if the given pointer is currently inside this pointable.</summary>
    public bool IsPointerInside(PointerInstance pointer) => _pointersInside.Contains(pointer);

    private List<PointerInstance> _pointersSelecting = new();
    /// <summary>True if the given pointer is currently selecting this pointable.</summary>
    public bool IsPointerSelecting(PointerInstance pointer) => _pointersSelecting.Contains(pointer);

    /// <summary>Returns true if the given point (in screen space) is inside this pointable.</summary>
    public abstract bool IsPointInside(Vector2 point);

    /// <summary>Registers this pointable with the <see cref="InputManager"/>.</summary>
    public Pointable()
    {
        InputManager.Instance!.RegisterPointable(this);
    }

    ~Pointable()
    {
        InputManager.Instance!.UnregisterPointable(this);
    }

    /// <summary>Called when a pointer enters this pointable. Raises <see cref="Entered"/>.</summary>
    public virtual void OnPointerEntered(PointerInstance pointer)
    {
        Dbg.Assert(!_pointersInside.Contains(pointer));
        _pointersInside.Add(pointer);

        Entered?.Invoke(pointer);
    }

    /// <summary>Called when a pointer exits this pointable. Raises <see cref="Exited"/>.</summary>
    public virtual void OnPointerExited(PointerInstance pointer)
    {
        Dbg.Assert(_pointersInside.Contains(pointer));
        _pointersInside.Remove(pointer);

        Exited?.Invoke(pointer);
    }

    /// <summary>Called when a pointer begins selecting this pointable. Raises <see cref="Selected"/>.</summary>
    public virtual void OnPointerSelected(PointerInstance pointer)
    {
        Dbg.Assert(!_pointersSelecting.Contains(pointer));
        _pointersSelecting.Add(pointer);

        Selected?.Invoke(pointer);
    }

    /// <summary>Called when a pointer stops selecting this pointable. Raises <see cref="Unselected"/>, and <see cref="Clicked"/> if the pointer is still inside.</summary>
    public virtual void OnPointerUnselected(PointerInstance pointer)
    {
        Dbg.Assert(_pointersSelecting.Contains(pointer));
        _pointersSelecting.Remove(pointer);

        Unselected?.Invoke(pointer);

        if (_pointersInside.Contains(pointer))
        {
            Clicked?.Invoke(pointer);
        }
    }
}
