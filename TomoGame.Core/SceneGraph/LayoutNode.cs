using Microsoft.Xna.Framework;
using System.Xml.Linq;
using TomoGame.Core.Sprites;

namespace TomoGame.Core.SceneGraph;

/// <summary>Builds its subtree from a layout file. Look up the named nodes in it with
/// <see cref="Node.FindNode"/>.</summary>
public class LayoutNode : Node
{
    public LayoutNode(string layoutFilePath, Node parent) : base(parent)
    {
        LocalSize = parent.LocalSize;
        LoadLayout(layoutFilePath);
    }

    private void LoadLayout(string layoutFilePath)
    {
        XDocument xmlDoc = XDocument.Load($"Content/{layoutFilePath}");
        XElement? root = xmlDoc.Root;
        if (!Dbg.Verify(root))
            return;

        // only to catch a name used twice in one layout, which FindNode would silently resolve to the first
        HashSet<string> names = [];

        Node parentNode = this;
        if (Dbg.Verify(root.Name.LocalName == "Layout"))
        {
            foreach (XElement child in root.Elements())
            {
                LoadElement(child, parentNode, names);
            }
        }
    }

    private void LoadElement(XElement element, Node parentNode, HashSet<string> names)
    {
        Node? newNode = LayoutNodeRegistry.CreateNode(element, parentNode);
        if (Dbg.Verify(newNode != null))
        {
            if (!string.IsNullOrEmpty(newNode.Name))
            {
                Dbg.Verify(names.Add(newNode.Name), $"layout has more than one node named '{newNode.Name}'");
            }
            parentNode = newNode!;
        }

        foreach (XElement child in element.Elements())
        {
            LoadElement(child, parentNode, names);
        }

        // after the children, so a node sizing to fit them sees them all, already fitted themselves
        newNode?.FinishLayout();
    }
}
