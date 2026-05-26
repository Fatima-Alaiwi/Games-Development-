using System.Collections.Generic;

public static class AudioTriggerTracker
{
    static readonly HashSet<string> _played = new HashSet<string>();

    public static void MarkPlayed(string id)
    {
        if (!string.IsNullOrEmpty(id)) _played.Add(id);
    }

    public static bool HasPlayed(string id) => _played.Contains(id);

    public static List<string> GetAll() => new List<string>(_played);

    public static void LoadFrom(List<string> ids)
    {
        _played.Clear();
        if (ids == null) return;
        foreach (var id in ids) _played.Add(id);
    }

    public static void Clear() => _played.Clear();
}
