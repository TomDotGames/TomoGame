using System.Collections;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace TomoGame.Core.Data;

/// <summary>Base for an entry in a <see cref="JsonData{T}"/> list, which stamps it with its load order.</summary>
public class JsonDataElement
{
    public const int InvalidId = -1;

    /// <summary>Position in the file. Data that cross-references entries can use this as a stable index.</summary>
    public int ID { get; private set; } = InvalidId;

    internal void SetID(int id)
    {
        Dbg.Assert(ID == InvalidId);
        ID = id;
    }
}

/// <summary>A list of <typeparamref name="T"/> deserialized from a JSON array, read off disk relative to the working directory.</summary>
public class JsonData<T> : IReadOnlyList<T> where T : JsonDataElement
{
    private readonly List<T> _data;

    public JsonData(string path)
    {
        using Stream stream = TitleContainer.OpenStream(path);
        using StreamReader reader = new StreamReader(stream);

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _data = JsonSerializer.Deserialize<List<T>>(reader.ReadToEnd(), options) ?? [];

        for (int id = 0; id < _data.Count; ++id)
        {
            _data[id].SetID(id);
        }
    }

    public int Count => _data.Count;

    public T this[int index] => _data[index];

    public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
