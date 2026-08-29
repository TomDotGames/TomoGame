namespace TomoGame.Core.Input;

/// <summary>Base class for a device that produces and updates one or more pointers.</summary>
internal abstract class PointerDevice
{
    private Dictionary<int, PointerInstance> _pointers = new();

    /// <summary>Adds a pointer to the device.</summary>
    protected void AddPointer(PointerInstance pointer)
    {
        Dbg.Assert(!_pointers.ContainsKey(pointer.ID));
        _pointers.Add(pointer.ID, pointer);
    }

    /// <summary>Removes a pointer from the device.</summary>
    protected void RemovePointer(PointerInstance pointer)
    {
        Dbg.Assert(_pointers.ContainsKey(pointer.ID));
        _pointers.Remove(pointer.ID);
    }

    /// <summary>Gets the pointer with the given id.</summary>
    public PointerInstance GetPointer(int id)
    {
        return _pointers[id];
    }

    /// <summary>Refreshes pointer state, then updates interactions against the given pointables.</summary>
    public void Update(List<Pointable> pointables)
    {
        UpdatePointers();
        foreach (PointerInstance pointer in _pointers.Values)
        {
            pointer.UpdateInteractions(pointables);
        }
    }

    /// <summary>Refreshes the device's pointer positions and selection states from the underlying hardware.</summary>
    protected abstract void UpdatePointers();
}
