using Microsoft.Xna.Framework;
using TomoGame.Core.Input;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.UI;

/// <summary>A clickable button. Shows a different sprite as the pointer enters, presses and leaves it, and
/// raises <see cref="Clicked"/> when a pointer is pressed and then released while still inside.
/// <para>Takes the name of a sprite sheet and expects it to define four sprites - <c>up</c>, <c>hover</c>,
/// <c>down</c> and <c>disabled</c> - so <c>"Sprites/Button"</c> resolves to <c>Sprites/Button.Up</c> and
/// friends.</para></summary>
public class Button : Node
{
    /// <summary>Which of the button's sprites is currently showing.</summary>
    public enum ButtonVisualState
    {
        /// <summary>Idle, with no pointer over it.</summary>
        Normal,
        /// <summary>A pointer is over the button but not pressing it.</summary>
        Hover,
        /// <summary>A pointer is pressing the button.</summary>
        Pressed,
        /// <summary>The button is not interactable.</summary>
        Disabled
    }

    private readonly string _spriteSheetName;
    private readonly SpriteNode _spriteNode;
    private readonly NodePointable _pointable;

    private ButtonVisualState _visualState = ButtonVisualState.Normal;

    /// <summary>Raised when a pointer is pressed on the button and released while still inside it.</summary>
    public event Action<Button>? Clicked;

    /// <summary>Which sprite the button is currently showing.</summary>
    public ButtonVisualState VisualState => _visualState;

    /// <summary>The button's hit area. Exposed so callers can adjust its <see cref="Pointable.Priority"/> when
    /// buttons overlap, or query which pointers are over it.</summary>
    public NodePointable Pointable => _pointable;

    /// <summary>When false the button shows its disabled sprite and ignores pointer input.</summary>
    public bool Interactable
    {
        get => _interactable;
        set
        {
            if (_interactable == value)
                return;

            _interactable = value;
            _pointable.Enabled = value;
            RefreshVisualState();
        }
    }
    private bool _interactable = true;

    /// <summary>Creates a button from a sprite sheet, sized to that sheet's <c>up</c> sprite. Call
    /// <see cref="Node.SetIntrinsicSize(Vector2)"/> to give it a different size; the sprite stretches to fit.</summary>
    public Button(string spriteSheetName, Node? parent = null) : base(parent)
    {
        _spriteSheetName = spriteSheetName;

        _spriteNode = new SpriteNode(SpriteNameFor(ButtonVisualState.Normal), Vector2.Zero, this);
        _spriteNode.Anchor = Vector2.Zero;
        _spriteNode.SetPositionInParentSpace(Vector2.Zero, Vector2.Zero);

        // start out the size of the sprite, which the caller is free to override afterwards
        SetIntrinsicSize(_spriteNode.IntrinsicSize);

        _pointable = new NodePointable(this);
        _pointable.Entered += _ => RefreshVisualState();
        _pointable.Exited += _ => RefreshVisualState();
        _pointable.Selected += _ => RefreshVisualState();
        _pointable.Unselected += _ => RefreshVisualState();
        _pointable.Clicked += _ => Clicked?.Invoke(this);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        // the sprite fills the button, so it has to follow whatever size the button is given
        if (_spriteNode.IntrinsicSize != IntrinsicSize)
            _spriteNode.SetIntrinsicSize(IntrinsicSize);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // without this the pointable lingers in the InputManager until it happens to be finalized,
        // still answering hit tests for a button that is no longer in the graph
        _pointable.Unregister();
    }

    private void RefreshVisualState()
    {
        ButtonVisualState state = GetVisualState();
        if (state == _visualState)
            return;

        _visualState = state;
        _spriteNode.SetSprite(SpriteNameFor(state));
    }

    private ButtonVisualState GetVisualState()
    {
        if (!_interactable)
            return ButtonVisualState.Disabled;

        // only show pressed while the pointer is still over the button, so dragging off it reads as a
        // release the same way the click itself does
        if (_pointable.IsAnyPointerSelecting && _pointable.IsAnyPointerInside)
            return ButtonVisualState.Pressed;

        return _pointable.IsAnyPointerInside ? ButtonVisualState.Hover : ButtonVisualState.Normal;
    }

    private string SpriteNameFor(ButtonVisualState state)
    {
        string suffix = state switch
        {
            ButtonVisualState.Hover => "Hover",
            ButtonVisualState.Pressed => "Down",
            ButtonVisualState.Disabled => "Disabled",
            _ => "Up"
        };

        return $"{_spriteSheetName}.{suffix}";
    }
}
