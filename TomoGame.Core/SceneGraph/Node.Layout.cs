using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.SceneGraph;

[LayoutNode("Transform")]
public partial class Node
{
    public string Name { get; private set; }

    public virtual void ApplyLayoutAttributes(XElement element)
    {
        // size and scale come first: pos anchors against LocalSize, which both of them change
        XAttribute? size = element.Attribute("size");
        if (size != null)
        {
            string[] tokens = size.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (Dbg.Verify(tokens.Length == 2, "size expects 2 values, eg: size=\"32 8\""))
            {
                IntrinsicSize = new Vector2(ParseFloat(tokens[0]), ParseFloat(tokens[1]));
            }
        }

        XAttribute? scale = element.Attribute("scale");
        if (scale != null)
        {
            LocalScale = ParseFloat(scale.Value);
        }

        XAttribute? rotation = element.Attribute("rotation");
        if (rotation != null)
        {
            LocalRotation = MathHelper.ToRadians(ParseFloat(rotation.Value));
        }

        XAttribute? pos = element.Attribute("pos");
        if (pos != null)
        {
            string[] tokens = pos.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (Dbg.Verify(tokens.Length == 4, "pos expects 4 values, eg: pos=\"cc cc 0 0\""))
            {
                OriginUV = AnchorPositionFromString(tokens[0]);

                Vector2 parentAnchorUV = AnchorPositionFromString(tokens[1]);
                // measured from the parent's origin, because that is where our LocalPosition starts from. Using
                // the parent's top left instead only agrees when the parent's own origin is its top left, which
                // is why this only shows up once something is nested under a node anchored anywhere else.
                Vector2 parentAnchor = Parent?.UVToChildSpace(parentAnchorUV) ?? Vector2.Zero;

                Vector2 offset = new Vector2(ParseFloat(tokens[2]), ParseFloat(tokens[3]));
                LocalPosition = parentAnchor + offset;
            }
        }
        
        XAttribute? name = element.Attribute("name");
        if (name != null)
        {
            Name = name.Value;
        }
    }

    /// <summary>Parses a number from a layout file. Layouts are authored with '.' as the decimal separator
    /// whatever the machine's locale is.</summary>
    protected static float ParseFloat(string value)
    {
        return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
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