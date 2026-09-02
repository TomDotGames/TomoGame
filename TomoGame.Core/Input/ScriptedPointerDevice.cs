using Microsoft.Xna.Framework;

namespace TomoGame.Core.Input;

/// <summary>A pointer driven by a script rather than by hardware, for exercising input without a desktop:
/// automated checks, demos, and repeatable repros.</summary>
/// <remarks>Steps are given in scene coordinates, so a script reads in the same units as the layout it is
/// poking at. At most one step runs per frame, because a press and a release in the same frame would collapse
/// into no interaction at all.</remarks>
public class ScriptedPointerDevice : PointerDevice
{
    private enum StepKind { Move, Press, Release, Wait, Invoke }

    private readonly record struct Step(StepKind Kind, Vector2 Position, float Seconds, Action? Action);

    private readonly PointerInstance _pointer = new();
    private readonly Queue<Step> _steps = new();

    private Vector2 _scenePosition;
    private bool _isSelecting;
    private float _waitRemaining;

    /// <summary>True once every step has run. The pointer then holds its last position and button state.</summary>
    public bool IsFinished => _steps.Count == 0 && _waitRemaining <= 0f;

    public ScriptedPointerDevice()
    {
        _pointer.ID = 0;
        AddPointer(_pointer);
    }

    public ScriptedPointerDevice MoveTo(Vector2 scenePosition) => Add(new Step(StepKind.Move, scenePosition, 0f, null));
    public ScriptedPointerDevice Press() => Add(new Step(StepKind.Press, Vector2.Zero, 0f, null));
    public ScriptedPointerDevice Release() => Add(new Step(StepKind.Release, Vector2.Zero, 0f, null));
    public ScriptedPointerDevice Wait(float seconds) => Add(new Step(StepKind.Wait, Vector2.Zero, seconds, null));

    /// <summary>Runs an action at this point in the script — capturing a frame, logging state, quitting.</summary>
    public ScriptedPointerDevice Do(Action action) => Add(new Step(StepKind.Invoke, Vector2.Zero, 0f, action));

    /// <summary>Presses and releases without moving in between, which is what raises Clicked.</summary>
    public ScriptedPointerDevice Click(Vector2 scenePosition)
    {
        return MoveTo(scenePosition).Press().Release();
    }

    /// <summary>Presses at one point, moves while held, and releases at the other. The pause after the press
    /// lets the game notice the pointer went down before it starts moving.</summary>
    public ScriptedPointerDevice Drag(Vector2 from, Vector2 to, float holdSeconds = 0.1f)
    {
        return MoveTo(from)
            .Press()
            .Wait(holdSeconds)
            .MoveTo(to)
            .Wait(holdSeconds)
            .Release();
    }

    private ScriptedPointerDevice Add(Step step)
    {
        _steps.Enqueue(step);
        return this;
    }

    protected override void UpdatePointers()
    {
        if (_waitRemaining > 0f)
        {
            _waitRemaining -= Time.TickSeconds;
        }
        else if (_steps.Count > 0)
        {
            RunStep(_steps.Dequeue());
        }

        _pointer.Position = GameBase.Instance?.ScenePositionToViewport(_scenePosition) ?? _scenePosition;

        // called every frame even when idle, so the pointer settles from JustSelected into Selecting and from
        // JustUnselected into NotSelecting
        _pointer.SetIsSelecting(_isSelecting);
    }

    private void RunStep(Step step)
    {
        switch (step.Kind)
        {
            case StepKind.Move:
                _scenePosition = step.Position;
                break;

            case StepKind.Press:
                _isSelecting = true;
                break;

            case StepKind.Release:
                _isSelecting = false;
                break;

            case StepKind.Wait:
                _waitRemaining = step.Seconds;
                break;

            case StepKind.Invoke:
                step.Action?.Invoke();
                break;
        }
    }
}
