using System.Collections.Generic;
using Faust.QoLChests.Configs;
using MiniMapLibrary;

namespace MapChests.Categories;

/// <summary>
/// Everything MapChests knows about one <see cref="MapCategory"/>. Highlighting, hiding and the minimap all read from here.
/// </summary>
public sealed class CategoryDefinition(
    MapCategory category,
    string group,
    string displayName,
    HighlightGroup? highlightGroup,
    InteractableKind minimapKind,
    IReadOnlyList<string> prefabPaths
)
{
    public MapCategory Category { get; } = category;

    /// <summary>Family the category belongs to (Chest, Shop, Shrine...); prefixes its config section so related sections sort together.</summary>
    public string Group { get; } = group;

    public string DisplayName { get; } = displayName;

    /// <summary>The original QoLChests group: null when the category is never highlighted. Supplies default colours and migration of old settings.</summary>
    public HighlightGroup? HighlightGroup { get; } = highlightGroup;

    /// <summary>The original minimap icon kind: supplies the default icon and size, and migration of old settings.</summary>
    public InteractableKind MinimapKind { get; } = minimapKind;

    /// <summary>Addressable prefab paths tagged with this category at startup.</summary>
    public IReadOnlyList<string> PrefabPaths { get; } = prefabPaths;

    public bool IsHighlightable => HighlightGroup.HasValue;

    /// <summary>Enemies, players, minions, items and neutrals: minimap only, never "used".</summary>
    public bool IsEntity => Category >= MapCategory.Enemy && Category <= MapCategory.Neutral;

    public string ConfigSection => $"{Group}: {DisplayName}";
}
