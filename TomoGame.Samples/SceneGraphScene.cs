using System;
using Microsoft.Xna.Framework;
using TomoGame.Core;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Samples;

/// <summary>Shows how a transform composes down the graph. The hierarchy is a layout file; the only thing
/// here is what drives it - two rotations and one scale, from which everything else follows.</summary>
public class SceneGraphScene : SceneRootNode
{
    private const float SystemSpinSpeed = 0.6f;   // radians per second
    private const float PlanetSpinSpeed = 2.2f;   // radians per second
    private const float PulseSpeed = 1.5f;        // radians per second
    private const float PulseAmount = 0.2f;       // keeps the dog's swing inside the scene at full stretch

    private readonly Node? _system;
    private readonly Node? _planet;

    public SceneGraphScene(GraphicsDeviceManager graphics, SceneScaleMode scaleMode, int size)
        : base(graphics, scaleMode, size)
    {
        LayoutNode layout = new LayoutNode("UI/SceneGraphSceneLayout.xml", this);

        _system = layout.FindNode("system");
        _planet = layout.FindNode("planet");
        Dbg.Verify(_system, "scene graph layout has no node named 'system'");
        Dbg.Verify(_planet, "scene graph layout has no node named 'planet'");
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        if (_system == null || _planet == null)
            return;

        // one rotation carries the house, the car and the dog; a second carries only the dog
        _system.LocalRotation = (Time.TotalSeconds * SystemSpinSpeed) % MathHelper.TwoPi;
        _planet.LocalRotation = (Time.TotalSeconds * PlanetSpinSpeed) % MathHelper.TwoPi;

        // scaling the planet scales the car and the dog with it, and widens the dog's orbit, from one call
        _planet.LocalScale = 1f + (PulseAmount * MathF.Sin(Time.TotalSeconds * PulseSpeed));

        // draw the graph itself. Node bounds would be misleading here: Rect is axis aligned, so a rotated
        // node's bounds do not line up with what is drawn.
        DrawHierarchy(_system);
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
}
