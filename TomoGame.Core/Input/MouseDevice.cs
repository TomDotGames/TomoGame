using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomoGame.Core.SceneGraph;

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

        // the mouse reports window pixels; everything downstream works in scene units
        Vector2 screenPosition = mouseState.Position.ToVector2();
        SceneRootNode? sceneRoot = GameBase.Instance?.SceneRoot;
        _pointer.Position = sceneRoot != null ? sceneRoot.ScreenToScene(screenPosition) : screenPosition;

        _pointer.SetIsSelecting(mouseState.LeftButton == ButtonState.Pressed);
    }
}
