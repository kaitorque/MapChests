using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;

namespace Faust.QoLChests.Compatability;

internal static class HunkCompatHandler
{
    public static void Register()
    {
        if (!HunkCompat.IsInstalled)
        {
            return;
        }

        InteractableRegistry.BlackList("HunkTerminal");
    }
}
