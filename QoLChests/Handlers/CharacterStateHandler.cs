namespace Faust.QoLChests.Handlers;

public static class CharacterStateHandler
{
    public static bool IsDrifter { get; private set; } = true;

    public static void SetIsDrifter(bool isDrifter)
    {
        IsDrifter = true;
    }
}