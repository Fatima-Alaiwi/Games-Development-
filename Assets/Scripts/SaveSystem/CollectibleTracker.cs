using System.Collections.Generic;

public static class CollectibleTracker
{
    static readonly HashSet<string> _collected = new HashSet<string>();

    public static void MarkCollected(string id)
    {
        if (!string.IsNullOrEmpty(id)) _collected.Add(id);
    }

    public static bool IsCollected(string id) => _collected.Contains(id);

    public static List<string> GetAll() => new List<string>(_collected);

    public static void LoadFrom(List<string> ids)
    {
        _collected.Clear();
        if (ids == null) return;
        foreach (var id in ids) _collected.Add(id);
    }

    public static void Clear() => _collected.Clear();
}
