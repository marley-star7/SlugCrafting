namespace SlugCrafting.Creatures;

public class AbstractCreatureCraftingData
{
    public List<int> scavengedBodyChunks = new List<int>();

    public WeakReference<Lizard> lizardRef { get; }

    public AbstractCreatureCraftingData(Lizard lizard)
    {
        lizardRef = new WeakReference<Lizard>(lizard);
    }
}

public static class LizardExtensions
{
}
