using System.Collections.Generic;
using Faust.QoLChests.Configs;

namespace Faust.QoLChests.Handlers;

public static class InteractableRegistry
{
    private static readonly Dictionary<string, InteractableCategory> RegisteredInteractables = [];
    private static readonly HashSet<string> BlackListedInteractables = [];

    /// <summary>
    /// Register an interactable by prefab name, to track them dynamically.
    /// </summary>
    /// <param name="interactableName"></param>
    public static void Register(string interactableName, InteractableCategory category)
    {
        RegisteredInteractables.TryAdd(interactableName, category);
        if (interactableName.EndsWith("(Clone)"))
            return;
        var cloneInteractableName = $"{interactableName}(Clone)";
        RegisteredInteractables.TryAdd(cloneInteractableName, category);
    }

    public static bool IsRegistered(string interactableName, out InteractableCategory category)
    {
        return RegisteredInteractables.TryGetValue(interactableName, out category);
    }

    public static void BlackList(string interactableName)
    {
        BlackListedInteractables.Add(interactableName);
        if (interactableName.EndsWith("(Clone)"))
            return;
        var cloneInteractableName = $"{interactableName}(Clone)";
        BlackListedInteractables.Add(cloneInteractableName);
    }

    public static bool IsBlackListed(string interactableName)
    {
        return BlackListedInteractables.Contains(interactableName);
    }
}
