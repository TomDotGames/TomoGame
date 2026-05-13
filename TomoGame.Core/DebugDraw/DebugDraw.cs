using Microsoft.Xna.Framework;
using TomoGame.Core.SceneGraph;

namespace TomoGame.Core;

/// <summary>Static helper for drawing debug visuals such as lines and rectangles.</summary>
public static class DebugDraw
{
    private static DebugDrawNode? _drawNode;

    private static void EnsureDrawNode()
    {
        SceneRootNode rootNode = GameBase.Instance!.SceneRoot!;
        if (_drawNode == null)
        {
            _drawNode = new DebugDrawNode(rootNode);
        }

        if (_drawNode.Parent != rootNode)
        {
            rootNode.AddChild(_drawNode);
        }
    }

    /// <summary>Draws a line from <paramref name="start"/> to <paramref name="end"/>.</summary>
    public static void Line(Vector2 start, Vector2 end, Color color, float thickness = 1f)
    {
        EnsureDrawNode();
        _drawNode!.AddLine(start, end, color, thickness);
    }

    /// <summary>Draws a rectangle.</summary>
    public static void Rect(Rect rect, Color color, float thickness = 1f)
    {
        EnsureDrawNode();
        _drawNode!.AddLine(rect.TopLeft, rect.TopRight, color, thickness);
        _drawNode!.AddLine(rect.TopRight, rect.BottomRight, color, thickness);
        _drawNode!.AddLine(rect.BottomRight, rect.BottomLeft, color, thickness);
        _drawNode!.AddLine(rect.BottomLeft, rect.TopLeft, color, thickness);
    }

    /// <summary>Draws the world rect of a <see cref="Node"/>, and optionally all of its children recursively.</summary>
    public static void NodeRect(Node node, Color color, float thickness = 1f, bool recursive = true)
    {
        EnsureDrawNode();

        Rect(node.WorldRect, color, thickness);
        if (!recursive)
            return;

        foreach (Node child in node.Children)
        {
            NodeRect(child, color, thickness, recursive);
        }
    }
}
