using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;
using TomoGame.UI;

namespace TomoGame.Samples;

/// <summary>Demonstrates <see cref="Button"/>: hover and press change the sprite, clicking adds or removes a
/// flower, and each button disables itself at the end of its range so the disabled sprite is on show too.</summary>
public class ButtonScene : SceneRootNode
{
    private const int MaxFlowers = 5;
    private static readonly Vector2 ButtonSize = new(24f, 6f);

    private readonly Button _addButton;
    private readonly Button _removeButton;
    private readonly List<SpriteNode> _flowers = [];

    public ButtonScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        _addButton = CreateButton(-8f);
        _addButton.Clicked += _ => AddFlower();

        _removeButton = CreateButton(4f);
        _removeButton.Clicked += _ => RemoveFlower();

        RefreshButtons();
    }

    /// <summary>Makes a button of a fixed size - the sprite stretches to fill it rather than staying at its
    /// own 32x8 - centred horizontally and offset from the middle of the scene.</summary>
    private Button CreateButton(float offsetY)
    {
        Button button = new Button("Sprites/Button", this);
        button.Anchor = new Vector2(0.5f, 0.5f);
        button.SetIntrinsicSize(ButtonSize);
        button.SetPositionInParentSpace(new Vector2(0.5f, 0.5f), new Vector2(0f, offsetY));
        return button;
    }

    private void AddFlower()
    {
        SpriteNode flower = new SpriteNode("Sprites/Samples.Flower", Vector2.Zero, this);
        flower.Anchor = new Vector2(0.5f, 1f);
        flower.SetPositionInParentSpace(new Vector2(0.5f, 1f), new Vector2(FlowerOffsetX(_flowers.Count), -6f));
        flower.PlayAnimation("wave", AnimationPlayer.AnimationMode.PingPong);

        _flowers.Add(flower);
        RefreshButtons();
    }

    private void RemoveFlower()
    {
        if (_flowers.Count == 0)
            return;

        // Destroy unlinks the node from the graph, and is safe to call from a click handler mid-frame
        SpriteNode flower = _flowers[^1];
        _flowers.RemoveAt(_flowers.Count - 1);
        flower.Destroy();

        RefreshButtons();
    }

    /// <summary>Spreads the flowers evenly either side of the scene's centre line.</summary>
    private static float FlowerOffsetX(int index)
    {
        return (index - ((MaxFlowers - 1) * 0.5f)) * 7f;
    }

    private void RefreshButtons()
    {
        _addButton.Interactable = _flowers.Count < MaxFlowers;
        _removeButton.Interactable = _flowers.Count > 0;
    }
}
