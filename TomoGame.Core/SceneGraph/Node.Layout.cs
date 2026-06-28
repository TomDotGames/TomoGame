using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

[LayoutNode("Transform")]
public partial class Node
{
    public virtual void ApplyLayoutAttributes(XElement element)
    {
        XAttribute? pos = element.Attribute("pos");
        if (pos != null)
        {
            string[] tokens = pos.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!Dbg.Verify(tokens.Length == 4))
                return;

            OriginUV = AnchorPositionFromString(tokens[0]);
            
            Vector2 parentAnchorUV = AnchorPositionFromString(tokens[1]);
            Vector2 parentAnchor = Parent?.UVToLocalSpace(parentAnchorUV) ?? Vector2.Zero;
            
            Vector2 offset = new Vector2(float.Parse(tokens[2]), float.Parse(tokens[3]));
            LocalPosition = parentAnchor + offset;
        }
    }

    private Vector2 AnchorPositionFromString(string anchorPos)
    {
        if (!Dbg.Verify(anchorPos.Length == 2))
            return Vector2.Zero;

        Dictionary<char, float> anchorMapY = new()
        {
            { 't', 0.0f },
            { 'c', 0.5f },
            { 'b', 1.0f }
        };
        if (!Dbg.Verify(anchorMapY.TryGetValue(anchorPos[0], out float yAnchor)))
            return Vector2.Zero;

        Dictionary<char, float> anchorMapX = new()
        {
            { 'l', 0.0f },
            { 'c', 0.5f },
            { 'r', 1.0f }
        };
        if (!Dbg.Verify(anchorMapX.TryGetValue(anchorPos[1], out float xAnchor)))
            return Vector2.Zero;

        return new Vector2(xAnchor, yAnchor);
    }
}