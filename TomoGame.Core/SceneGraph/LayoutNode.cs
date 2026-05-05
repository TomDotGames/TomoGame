using Microsoft.Xna.Framework;
using System.Xml.Linq;
using TomoGame.Core.Sprites;

namespace TomoGame.Core.SceneGraph;

public class LayoutNode : TransformNode
{
    public LayoutNode(string layoutFilePath, Node parent) : base(Vector2.Zero, Vector2.Zero, parent)
    {
        TransformNode? parentTransformNode = parent as TransformNode;
        if (Dbg.Verify(parentTransformNode != null))
        {
            SetSize(parentTransformNode!.WorldRect.Size);
        }
        
        LoadLayout(layoutFilePath);
    }

    private void LoadLayout(string layoutFilePath)
    {
        XDocument xmlDoc = XDocument.Load($"Content/{layoutFilePath}");
        XElement root = xmlDoc.Root;
        TransformNode parentTransformNode = this;
        if (Dbg.Verify(root.Name.LocalName == "Layout"))
        {
            foreach (XElement child in root.Elements())
            {
                LoadElement(child, parentTransformNode);
            }
        }
    }

    private void LoadElement(XElement element, TransformNode parentTransformNode)
    {
        TransformNode? newNode = LayoutNodeRegistry.CreateNode(element, parentTransformNode);
        if (Dbg.Verify(newNode != null))
        {
            parentTransformNode = newNode!;
        }
        
        foreach (XElement child in element.Elements())
        {
            LoadElement(child, parentTransformNode);
        }
    }
}