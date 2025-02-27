using Faust.QoLChests.Handlers;

internal static class HunkCompatHandler
{
    public static void Register()
    {
        InteractableRegistry.BlackList("HunkTerminal");
    }
}
