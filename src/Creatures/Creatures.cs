namespace SlugCrafting.CreaturesExtension;

public class Creatures
{
    public static void Remove()
    {
        GarbageWormHooks.RemoveHooks();
    }

    public static void Apply()
    {
        Plugin.LogInfo("Start CreatureExtension");

        GarbageWormHooks.ApplyHooks();

        Plugin.LogInfo("End CreatureExtension");
    }
}
