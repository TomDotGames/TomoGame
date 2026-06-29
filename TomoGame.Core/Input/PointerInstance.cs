using Microsoft.Xna.Framework;

namespace TomoGame.Core.Input;

/// <summary>The selection (press) state of a pointer over a frame.</summary>
public enum SelectingState
{
    /// <summary>The pointer is not selecting.</summary>
    NotSelecting,
    /// <summary>The pointer began selecting this frame.</summary>
    JustSelected,
    /// <summary>The pointer has been selecting for more than one frame.</summary>
    Selecting,
    /// <summary>The pointer stopped selecting this frame.</summary>
    JustUnselected
}

/// <summary>A single pointer (e.g. mouse cursor or touch point) with a position and selection state.</summary>
public class PointerInstance
{
    /// <summary>The unique id of this pointer.</summary>
    public int ID { get; set; }

    /// <summary>The pointer's position in screen space.</summary>
    public Vector2 Position { get; set; }

    /// <summary>This pointer's current selection state.</summary>
    public SelectingState SelectingState => _selectingState;
    private SelectingState _selectingState = SelectingState.NotSelecting;

    /// <summary>True while the pointer is selecting (either just started or continuing).</summary>
    public bool IsSelecting => SelectingState == SelectingState.JustSelected ||
                               SelectingState == SelectingState.Selecting;

    /// <summary>Updates the selection state from a raw selecting flag, deriving the just/continuing transitions.</summary>
    public void SetIsSelecting(bool isSelecting)
    {
        if (isSelecting)
        {
            _selectingState = IsSelecting ? SelectingState.Selecting : SelectingState.JustSelected;
        }
        else
        {
            _selectingState = IsSelecting ? SelectingState.JustUnselected : SelectingState.NotSelecting;
        }
    }

    /// <summary>Resolves which pointables this pointer can interact with and raises their enter/exit/select events.</summary>
    public void UpdateInteractions(List<Pointable> pointables)
    {
        // TODO: this is a bunch of work and allocations for something that should be pretty simple..

        // get a list of all pointables that this pointer is inside
        List<Pointable> pointablesOver = new();
        List<Pointable> nonInteractablePointables = new();

        Vector2 scenePosition = GameBase.Instance.ViewportToScenePosition(Position);
        foreach (Pointable pointable in pointables)
        {
            if (pointable.IsPointInside(scenePosition))
            {
                pointablesOver.Add(pointable);
            }
            else
            {
                nonInteractablePointables.Add(pointable);
            }
        }

        // now figure out which exclusive pointable we can interact with
        Pointable? pointableTopExclusive = null;
        float topExclusivePriority = float.MinValue;
        foreach (Pointable pointable in pointablesOver)
        {
            if (pointable.IsExclusive && pointable.Priority > topExclusivePriority)
            {
                topExclusivePriority = pointable.Priority;
                pointableTopExclusive = pointable;
            }
        }

        List<Pointable> interactablePointables = new();
        if (pointableTopExclusive != null)
        {
            interactablePointables.Add(pointableTopExclusive);
        }

        // which non-exclusives can we interact with?
        foreach (Pointable pointable in pointablesOver)
        {
            if (!pointable.IsExclusive && pointable.Priority > topExclusivePriority)
            {
                interactablePointables.Add(pointable);
            }
            else if (pointable != pointableTopExclusive)
            {
                nonInteractablePointables.Add(pointable);
            }
        }

        Dbg.Assert(pointables.Count == interactablePointables.Count + nonInteractablePointables.Count);

        // update those pointables that we are able to interact with
        foreach (Pointable pointable in interactablePointables)
        {
            if (!pointable.IsPointerInside(this))
            {
                pointable.OnPointerEntered(this);
            }

            // update selecting state
            if (SelectingState == SelectingState.JustSelected)
            {
                pointable.OnPointerSelected(this);
            }
            else if (SelectingState == SelectingState.JustUnselected &&
                     pointable.IsPointerSelecting(this))
            {
                pointable.OnPointerUnselected(this);
            }
        }

        // update those pointables that we are not able to interact with
        foreach (Pointable pointable in nonInteractablePointables)
        {
            if (pointable.IsPointerInside(this))
            {
                pointable.OnPointerExited(this);
            }

            // update selecting state
            if (SelectingState == SelectingState.JustUnselected &&
                pointable.IsPointerSelecting(this))
            {
                pointable.OnPointerUnselected(this);
            }
        }
    }
}
