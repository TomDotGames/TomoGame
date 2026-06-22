using Microsoft.Xna.Framework.Input;

namespace TomoGame.Core.Input;

/// <summary>A <see cref="PointerDevice"/> backed by the system mouse, exposing a single pointer.</summary>
internal class MouseDevice : PointerDevice
{
    private PointerInstance _pointer;

    public MouseDevice()
    {
        _pointer = new PointerInstance();
        _pointer.ID = 0;
        AddPointer(_pointer);
    }

    protected override void UpdatePointers()
    {
        MouseState mouseState = Mouse.GetState();
        _pointer.Position = mouseState.Position.ToVector2();
        _pointer.SetIsSelecting(mouseState.LeftButton == ButtonState.Pressed);
    }
}
