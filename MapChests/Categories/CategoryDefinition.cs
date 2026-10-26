using System.Collections.Generic;
using Faust.QoLChests.Configs;
using MiniMapLibrary;

namespace MapChests.Categories;

/// <summary>
/// Everything MapChests knows about one <see cref="MapCategory"/>. Highlighting, hiding and the minimap all read from here.
/// </summary>
public sealed class CategoryDefinition(
    MapCategory category,
    string displayName,
    HighlightGroup? highlightGroup,
    InteractableKind? minimapKind,
    IReadOnlyList<string> prefabPaths
)
{
    public MapCategory Category { get; } = category;

    public string DisplayName { get; } = displayName;

    /// <summary>Highlight and hide config entries used for this category; null when it is never highlighted.</summary>
    public HighlightGroup? HighlightGroup { get; } = highlightGroup;

    /// <summary>Minimap icon settings used for this category; null when it is not shown on the minimap.</summary>
    public InteractableKind? MinimapKind { get; } = minimapKind;

    /// <summary>Addressable prefab paths tagged with this category at startup.</summary>
    public IReadOnlyList<string> PrefabPaths { get; } = prefabPaths;
}
