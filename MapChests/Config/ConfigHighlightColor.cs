using UnityEngine;

namespace Faust.QoLChests.Configs;

public enum ConfigHighlightColor
{
    Yellow = 0,
    Red = 1,
    Blue = 2,
    Pink = 3,
    Orange = 4,
    Green = 5,
    Cyan = 6,
    Violet = 7,
    Magenta = 8,
    Gray = 9,
    White = 10
}

public static class ConfigHighlightColors
{
    public static Color32 ToColor32(this ConfigHighlightColor color) =>
        color switch
        {
            ConfigHighlightColor.Yellow => new(255, 255, 1, 255),
            ConfigHighlightColor.Red => new(255, 1, 1, 255),
            ConfigHighlightColor.Blue => new(1, 1, 255, 255),
            ConfigHighlightColor.Pink => new(255, 1, 127, 255),
            ConfigHighlightColor.Orange => new(255, 128, 1, 255),
            ConfigHighlightColor.Green => new(1, 255, 1, 255),
            ConfigHighlightColor.Cyan => new(1, 255, 255, 255),
            ConfigHighlightColor.Violet => new(127, 1, 255, 255),
            ConfigHighlightColor.Magenta => new(255, 1, 255, 255),
            ConfigHighlightColor.Gray => new(128, 128, 128, 255),
            ConfigHighlightColor.White => new(255, 255, 255, 255),
            _ => new(255, 255, 1, 255)
        };
}
