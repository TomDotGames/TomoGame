using Microsoft.Xna.Framework;
using System.Xml.Linq;
using TomoGame.Core.Sprites;

namespace TomoGame.Core.SceneGraph;

public class LayoutNode : Node
{
    public LayoutNode(string layoutFilePath, Node parent) : base(Vector2.Zero, Vector2.Zero, parent)
    {
        SetSize(parent.WorldRect.Size);
        LoadLayout(layoutFilePath);
    }

    private void LoadLayout(string layoutFilePath)
    {
        XDocument xmlDoc = XDocument.Load($"Content/{layoutFilePath}");
        XElement root = xmlDoc.Root;
        Node parentNode = this;
        if (Dbg.Verify(root.Name.LocalName == "Layout"))
        {
            foreach (XElement child in root.Elements())
            {
                LoadElement(child, parentNode);
            }
        }
    }

    private void LoadElement(XElement element, Node parentNode)
    {
        Node? newNode = LayoutNodeRegistry.CreateNode(element, parentNode);
        if (Dbg.Verify(newNode != null))
        {
            parentNode = newNode!;
        }

        foreach (XElement child in element.Elements())
        {
            LoadElement(child, parentNode);
        }
    }
}