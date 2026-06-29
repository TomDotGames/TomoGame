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

        // make sure all referenced assemblies are loaded
        string coreName = typeof(Node).Assembly.GetName().Name!; // "TomoGame.Core"

        foreach (string dll in Directory.GetFiles(AppContext.BaseDirectory, "TomoGame*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(dll);
            }
            catch (Exception ex) when (ex is BadImageFormatException or FileLoadException)
            {
                continue;
            }

            bool relevant = assembly.GetName().Name == coreName
                            || assembly.GetReferencedAssemblies().Any(r => r.Name == coreName);
            if (relevant)
            {
                RegisterAssembly(assembly);
            }
        }
        
        _initialized = true;
    }
    
    private static void RegisterAssembly(Assembly assembly)
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

    public static Node? CreateNode(XElement element, Node parent)
    {
        if (!_initialized)
            Initialize();

        _nodeCreators.TryGetValue(element.Name.LocalName, out Func<Node?, Node>? creator);
        if (Dbg.Verify(creator))
        {
            Node node = creator(parent);
            node.ApplyLayoutAttributes(element);
            return node;
        }

        return null;
    }
}