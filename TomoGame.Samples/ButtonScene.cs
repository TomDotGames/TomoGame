using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Resources;
using TomoGame.Core.Sprites;
using TomoGame.Core.Text;
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
    private readonly Label _countLabel;
    private readonly List<SpriteNode> _flowers = [];

    public ButtonScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        BitmapFont font = ResourceManager.Instance!.GetFont("Fonts/Tiny");

        _addButton = CreateButton(-8f, "ADD", font);
        _addButton.Clicked += _ => AddFlower();

        _removeButton = CreateButton(4f, "REMOVE", font);
        _removeButton.Clicked += _ => RemoveFlower();

        // a label is just a node, so it anchors into the scene like anything else
        _countLabel = new Label(string.Empty, font, 2.5f, this);
        _countLabel.Anchor = new Vector2(0.5f, 0f);
        _countLabel.SetPositionInParentSpace(new Vector2(0.5f, 0f), new Vector2(0f, 4f));

        RefreshButtons();
    }

    /// <summary>Makes a button of a fixed size - the sprite stretches to fill it rather than staying at its
    /// own 32x8 - centred horizontally and offset from the middle of the scene.</summary>
    private Button CreateButton(float offsetY, string caption, BitmapFont font)
    {
        Button button = new Button("Sprites/Button", this);
        button.Anchor = new Vector2(0.5f, 0.5f);
        button.SetIntrinsicSize(ButtonSize);
        button.SetPositionInParentSpace(new Vector2(0.5f, 0.5f), new Vector2(0f, offsetY));

        // parenting the caption to the button means it inherits the button's position, and draws over the
        // button's sprite because it was added to the graph after it
        Label label = new Label(caption, font, 3f, button);
        label.Anchor = new Vector2(0.5f, 0.5f);
        label.SetPositionInParentSpace(new Vector2(0.5f, 0.5f), Vector2.Zero);
        label.Color = Color.White;

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
        _countLabel.Text = $"FLOWERS: {_flowers.Count}";
    }
}
