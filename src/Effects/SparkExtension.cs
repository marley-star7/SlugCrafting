using SlugCrafting.Creatures;

namespace SlugCrafting.Effects;

public class SparkCraftingData
{
    public WeakReference<Spark> sparkRef;

    public LizardShellColorGraphics lizardShellColorGraphics;

    public SparkCraftingData(Spark spark)
    {
        this.sparkRef = new WeakReference<Spark>(spark);
    }
}

public static class SparkCraftingExtension
{
    private static readonly ConditionalWeakTable<Spark, SparkCraftingData> conditionalWeakTable = new();

    public static SparkCraftingData GetSparkCraftingData(this Spark spark)
    {
        return conditionalWeakTable.GetValue(spark, _ => new SparkCraftingData(spark));
    }
}
