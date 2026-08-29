using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.Samples;

/// <summary>Shows how transforms compose down the scene graph. A house drifts around the scene, pulsing its
/// scale and turning slowly on the spot, while a dog orbits it several times faster and spins about its own
/// middle as it goes. The three rates are independent because they are driven by separate nodes: the house
/// spins itself, an empty pivot node sweeps the dog around, and the dog adds its own spin on top of that
/// sweep. All of them still inherit the system's movement and scale.</summary>
public class SceneGraphComplexScene : SceneRootNode
{
    private const float DriftSpeed = 5f;        // scene units per second
    private const float HouseSpinSpeed = 0.5f;  // radians per second
    private const float OrbitSpeed = 2.2f;      // radians per second
    private const float DogSpinSpeed = 3f;      // radians per second, on top of the orbit sweep it inherits
    private const float PulseSpeed = 1.8f;      // radians per second
    private const float BaseScale = 0.65f;      // keeps the whole system inside the scene at full stretch
    private const float PulseAmount = 0.25f;    // scale swings between BaseScale * (1 +/- this)
    private const float OrbitRadius = 14f;      // in the system's own space, so the drawn orbit is this * its world scale

    /// <summary>How close the system's anchor may get to the scene edge, so the dog stays on screen at full scale.</summary>
    private const float DriftMargin = 14f;

    /// <summary>Empty node carrying the movement and scale that everything else inherits.</summary>
    private readonly Node _system;

    /// <summary>Empty node whose only job is to spin, sweeping whatever hangs off it around <see cref="_system"/>.</summary>
    private readonly Node _orbit;

    private readonly SpriteNode _house;
    private readonly SpriteNode _dog;

    // normalised so the diagonal drift runs at DriftSpeed rather than faster than it
    private Vector2 _driftDirection = Vector2.Normalize(new Vector2(1f, 1.6f));

    public SceneGraphComplexScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        // the system has no size of its own, so it is just a point to hang the rest off - its anchor is
        // wherever it sits, and moving or scaling it carries the house and the dog's orbit along with it
        _system = new Node(Vector2.Zero, Vector2.Zero, this);
        _system.SetPositionInParentSpace(new Vector2(0.5f, 0.5f), Vector2.Zero); // start at the scene's centre

        // anchored at its middle so it turns on the spot rather than about a corner. it sits on the system's
        // point, and rotating it turns only itself - it is not in the dog's parent chain.
        _house = new SpriteNode("Sprites/Samples.House", Vector2.Zero, _system);
        _house.Anchor = new Vector2(0.5f, 0.5f);
        _house.SetPositionInParentSpace(Vector2.Zero, Vector2.Zero);

        // a sibling of the house rather than a child of it, so its spin rate is its own
        _orbit = new Node(Vector2.Zero, Vector2.Zero, _system);
        _orbit.SetPositionInParentSpace(Vector2.Zero, Vector2.Zero);

        // offset from the orbit node's pivot, so spinning that node sweeps the dog around the system
        _dog = new SpriteNode("Sprites/Samples.Dog", Vector2.Zero, _orbit);
        _dog.Anchor = new Vector2(0.5f, 0.5f);
        _dog.SetPositionInParentSpace(Vector2.Zero, new Vector2(OrbitRadius, 0f));
        _dog.PlayAnimation("wag", AnimationPlayer.AnimationMode.Loop);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        DriftSystem();

        // scaling the system scales the house, the dog, and the radius of the orbit, all from one call
        float pulse = 1f + (PulseAmount * MathF.Sin(Time.TotalSeconds * PulseSpeed));
        _system.SetLocalScale(BaseScale * pulse);

        // three independent rates: the house turns on the spot, the empty pivot sweeps the dog around faster,
        // and the dog turns about its own anchor as well. rotation accumulates down the graph, so the dog's
        // WorldRotation ends up as the orbit's sweep plus its own spin.
        _house.SetLocalRotation((Time.TotalSeconds * HouseSpinSpeed) % MathHelper.TwoPi);
        _orbit.SetLocalRotation((Time.TotalSeconds * OrbitSpeed) % MathHelper.TwoPi);
        _dog.SetLocalRotation((Time.TotalSeconds * DogSpinSpeed) % MathHelper.TwoPi);
    }

    private void DriftSystem()
    {
        _system.Translate(_driftDirection * (Time.TickSeconds * DriftSpeed));

        // WorldAnchorPosition is the pivot, so it is unaffected by any spin or scale below it
        Vector2 pivot = _system.WorldAnchorPosition;

        if ((_driftDirection.X > 0f && pivot.X > WorldRect.Width - DriftMargin) ||
            (_driftDirection.X < 0f && pivot.X < DriftMargin))
            _driftDirection.X = -_driftDirection.X;

        if ((_driftDirection.Y > 0f && pivot.Y > WorldRect.Height - DriftMargin) ||
            (_driftDirection.Y < 0f && pivot.Y < DriftMargin))
            _driftDirection.Y = -_driftDirection.Y;
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);

        // draw the orbit arm between the two anchors. WorldRect is axis-aligned and so is not meaningful
        // for a rotated node, but the anchor positions are exact whatever the transform is doing.
        DebugDraw.Line(_house.WorldAnchorPosition, _dog.WorldAnchorPosition, Color.White * 0.5f, 0.15f);
    }
}
