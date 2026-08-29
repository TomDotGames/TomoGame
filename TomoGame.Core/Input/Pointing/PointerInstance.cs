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

    // reused between passes so dispatch allocates nothing after the first frame, and so it runs over a
    // snapshot: a handler may register or unregister pointables - destroying a button from its own click
    // handler does exactly that - and must not disturb the pass it was raised from
    private readonly List<Pointable> _candidates = [];
    private readonly List<bool> _candidateIsInside = [];

    /// <summary>Resolves which pointables this pointer can interact with and raises their enter/exit/select events.</summary>
    public void UpdateInteractions(List<Pointable> pointables)
    {
        _candidates.Clear();
        _candidateIsInside.Clear();
        foreach (Pointable pointable in pointables)
        {
            _candidates.Add(pointable);
            _candidateIsInside.Add(pointable.IsPointInside(Position));
        }

        // an exclusive pointable takes the pointer from everything at or below its priority
        Pointable? topExclusive = null;
        float topExclusivePriority = float.MinValue;
        for (int i = 0; i < _candidates.Count; i++)
        {
            Pointable pointable = _candidates[i];
            if (_candidateIsInside[i] && pointable.IsExclusive && pointable.Priority > topExclusivePriority)
            {
                topExclusivePriority = pointable.Priority;
                topExclusive = pointable;
            }
        }

        for (int i = 0; i < _candidates.Count; i++)
        {
            Pointable pointable = _candidates[i];

            // the top exclusive, plus any non-exclusive ranked above it, take the pointer; everything else,
            // whether or not the pointer is over it, is treated as having lost the pointer
            bool interacting = pointable == topExclusive ||
                               (_candidateIsInside[i] && !pointable.IsExclusive &&
                                pointable.Priority > topExclusivePriority);

            if (interacting && !pointable.IsPointerInside(this))
                pointable.OnPointerEntered(this);
            else if (!interacting && pointable.IsPointerInside(this))
                pointable.OnPointerExited(this);

            if (interacting && SelectingState == SelectingState.JustSelected)
                pointable.OnPointerSelected(this);
            else if (SelectingState == SelectingState.JustUnselected && pointable.IsPointerSelecting(this))
                pointable.OnPointerUnselected(this);
        }
    }
}
