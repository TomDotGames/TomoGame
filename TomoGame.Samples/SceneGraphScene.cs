using System;
using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;
using TomoGame.Core.Sprites;

namespace TomoGame.Samples;

/// <summary>Shows how a transform composes down the graph. Nothing here is positioned by hand every frame:
/// one node turns, and everything under it is carried around by that, including a second level that turns at
/// a rate of its own. The node bounds are drawn so the hierarchy is visible.</summary>
public class SceneGraphScene : SceneRootNode
{
    private const float SystemSpinSpeed = 0.6f;   // radians per second
    private const float MoonSpinSpeed = 2.2f;     // radians per second
    private const float PulseSpeed = 1.5f;        // radians per second
    private const float PulseAmount = 0.2f;   // keeps the dog's swing inside the scene at full stretch
    private const float OrbitRadius = 11f;
    private const float MoonOrbitRadius = 4f;
    private const float HouseScale = 0.7f;    // small enough that the car clears it

    /// <summary>Turns, and so sweeps everything under it around the centre of the scene.</summary>
    private readonly Node _system;

    /// <summary>Sits on the system's own pivot, so the system's turn spins it on the spot rather than
    /// carrying it anywhere.</summary>
    private readonly SpriteNode _house;

    /// <summary>Carried around by the system, and turning at its own rate so its own child orbits it.</summary>
    private readonly Node _planet;

    public SceneGraphScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        // an empty node with no size of its own, just a point to hang the rest off
        _system = new Node(new Vector2(20f, 30f), this);

        _house = new SpriteNode("Sprites/Samples.House", Vector2.Zero, _system);
        _house.OriginUV = new Vector2(0.5f, 0.5f);
        _house.LocalScale = HouseScale;

        // one orbit radius out from the system, so turning the system sweeps it around the house
        _planet = new Node(new Vector2(OrbitRadius, 0f), _system);

        SpriteNode car = new SpriteNode("Sprites/Samples.Car", Vector2.Zero, _planet);
        car.OriginUV = new Vector2(0.5f, 0.5f);

        // a second level: carried by the planet, and orbiting it because the planet turns as well
        SpriteNode dog = new SpriteNode("Sprites/Samples.Dog", new Vector2(MoonOrbitRadius, 0f), _planet);
        dog.OriginUV = new Vector2(0.5f, 0.5f);
        dog.PlayAnimation("wag", AnimationPlayer.AnimationMode.Loop);
    }

    /// <summary>Draws a line along every parent-child link, which is the hierarchy made visible.</summary>
    private static void DrawHierarchy(Node node)
    {
        foreach (Node child in node.Children)
        {
            DebugDraw.Line(node.WorldPosition, child.WorldPosition, Color.BlueViolet, 0.3f);
            DrawHierarchy(child);
        }
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        // one rotation carries the house, the car and the dog; a second carries only the dog
        _system.LocalRotation = (Time.TotalSeconds * SystemSpinSpeed) % MathHelper.TwoPi;
        _planet.LocalRotation = (Time.TotalSeconds * MoonSpinSpeed) % MathHelper.TwoPi;

        // scaling the planet scales the car and the dog with it, and widens the dog's orbit, from one call
        _planet.LocalScale = 1f + (PulseAmount * MathF.Sin(Time.TotalSeconds * PulseSpeed));

        // draw the graph itself. Node bounds would be misleading here: Rect is axis aligned, so a rotated
        // node's bounds do not line up with what is drawn.
        DrawHierarchy(_system);
    }
}
