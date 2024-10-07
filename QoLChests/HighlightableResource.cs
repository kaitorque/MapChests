using System.Linq;
using Faust.QoLChests.Configs;

namespace Faust.QoLChests;

public record struct HighlightableResource(
    string GameObjectName,
    string ResourcePath,
    InteractableCategory Category
)
{
    public static HighlightableResource CreateFromResourcePath(
        string resourcePath,
        InteractableCategory category
    )
    {
        return new HighlightableResource(
            resourcePath.Split('/').Last().Replace(".prefab", ""),
            resourcePath,
            category
        );
    }
};
