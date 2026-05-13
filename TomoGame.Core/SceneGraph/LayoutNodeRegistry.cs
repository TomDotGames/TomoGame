using System.Reflection;
using System.Xml.Linq;

namespace TomoGame.Core.SceneGraph;

[AttributeUsage(AttributeTargets.Class)]
public class LayoutNodeAttribute : Attribute
{
    public string Name { get; }
    public LayoutNodeAttribute(string name) => Name = name;
}

public static class LayoutNodeRegistry
{
    private static Dictionary<string, Func<Node?, Node>> _nodeCreators = new();
    private static bool _initialized = false;

    private static void Initialize()
    {
        if (_initialized) return;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                LayoutNodeAttribute? attribute = type.GetCustomAttribute<LayoutNodeAttribute>();
                if (attribute != null)
                {
                    _nodeCreators[attribute.Name] = (parent) =>
                        (Node)Activator.CreateInstance(
                            type,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                            binder: null,
                            args: new object?[] { parent },
                            culture: null)!;
                }
            }
        }

        _initialized = true;
    }

    public static Node? CreateNode(XElement element, Node parent)
    {
        if (!_initialized)
            Initialize();

        if (Dbg.Verify(_nodeCreators.TryGetValue(
                element.Name.LocalName,
                out Func<Node?, Node>? creator
                )))
        {
            Node node = creator(parent);
            node.ApplyLayoutAttributes(element);
            return node;
        }

        return null;
    }
}