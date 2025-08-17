namespace SlugCrafting.Creatures;

public class AbstractCreatureCraftingData
{
    public List<int> scavengedBodyChunks = new List<int>();

    public WeakReference<AbstractCreature> abstractCreatureRef { get; }

    public AbstractCreatureCraftingData(AbstractCreature abstractCreature)
    {
        abstractCreatureRef = new WeakReference<AbstractCreature>(abstractCreature);
    }
}

public static class LizardExtensions
{
    private static readonly ConditionalWeakTable<AbstractCreature, AbstractCreatureCraftingData> _craftingDataTable = new();

    public static AbstractCreatureCraftingData GetAbstractCreatureCraftingData(this AbstractCreature abstractCreaute) =>
        _craftingDataTable.GetValue(abstractCreaute, _ => new AbstractCreatureCraftingData(_));
}
