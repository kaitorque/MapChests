using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;

namespace Faust.QoLChests.Configs;

/// <summary>
/// Read-only view of the original QoLChests and minimap config files, used once to seed <c>kaitorque.MapChests.cfg</c>.
/// Prefers the Phase 0 copies (<c>kaitorque.MapChests.Highlight.cfg</c>, <c>kaitorque.MapChests.Minimap.cfg</c>) over the originals.
/// </summary>
public sealed class LegacyConfig
{
    private readonly Dictionary<string, string> highlight;
    private readonly Dictionary<string, string> minimap;

    private LegacyConfig(Dictionary<string, string> highlight, Dictionary<string, string> minimap)
    {
        this.highlight = highlight;
        this.minimap = minimap;
    }

    public static LegacyConfig Load() => new(
        Read("kaitorque.MapChests.Highlight.cfg", "Faust.QoLChests.cfg"),
        Read("kaitorque.MapChests.Minimap.cfg", "MiniMap.cfg"));

    public static LegacyConfig Empty() => new([], []);

    public bool HighlightBool(string section, string key, bool fallback) =>
        highlight.TryGetValue(Key(section, key), out var value) && bool.TryParse(value, out var parsed) ? parsed : fallback;

    public float HighlightFloat(string section, string key, float fallback) =>
        highlight.TryGetValue(Key(section, key), out var value) && float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;

    public Color HighlightPaletteColor(string section, string key, Color fallback) =>
        highlight.TryGetValue(Key(section, key), out var value) && Enum.TryParse<ConfigHighlightColor>(value, true, out var parsed) ? parsed.ToColor32() : fallback;

    public bool MinimapBool(string section, string key, bool fallback) =>
        minimap.TryGetValue(Key(section, key), out var value) && bool.TryParse(value, out var parsed) ? parsed : fallback;

    public float MinimapFloat(string section, string key, float fallback) =>
        minimap.TryGetValue(Key(section, key), out var value) && float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;

    public string MinimapString(string section, string key, string fallback) =>
        minimap.TryGetValue(Key(section, key), out var value) && value.Length > 0 ? value : fallback;

    public Color MinimapColor(string section, string key, Color fallback) =>
        minimap.TryGetValue(Key(section, key), out var value) && ColorUtility.TryParseHtmlString("#" + value, out var parsed) ? parsed : fallback;

    public Vector2 MinimapVector2(string section, string key, Vector2 fallback)
    {
        // BepInEx writes Vector2 as {"x":1.0,"y":2.0}
        if (!minimap.TryGetValue(Key(section, key), out var value))
            return fallback;
        try
        {
            return JsonUtility.FromJson<Vector2>(value);
        }
        catch (Exception)
        {
            return fallback;
        }
    }

    public T MinimapEnum<T>(string section, string key, T fallback) where T : struct, Enum =>
        minimap.TryGetValue(Key(section, key), out var value) && Enum.TryParse<T>(value, true, out var parsed) ? parsed : fallback;

    private static string Key(string section, string key) => section + "\n" + key;

    private static Dictionary<string, string> Read(params string[] fileNames)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var fileName in fileNames)
        {
            var path = Path.Combine(Paths.ConfigPath, fileName);
            if (!File.Exists(path))
                continue;

            var section = "";
            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                    continue;
                }
                var separator = line.IndexOf('=');
                if (separator > 0)
                    values[Key(section, line.Substring(0, separator).Trim())] = line.Substring(separator + 1).Trim();
            }
            return values;
        }
        return values;
    }
}
