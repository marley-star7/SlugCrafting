using System.Runtime.CompilerServices;

namespace SlugCrafting.Creatures
{
    /// <summary>
    /// Saves mainly the sprite leaser for each lizard in the lizard's graphics,
    /// So we can reference later when stealing their heads via scavenging!
    /// </summary>
    public class LizardGraphicsData
    {
        public RoomCamera.SpriteLeaser spriteLeaser;
    }

    public static class LizardGraphicsExtension
    {
        private static readonly ConditionalWeakTable<LizardGraphics, LizardGraphicsData> lizardsGraphicsStorage = new();

        public static LizardGraphicsData GetGraphicsData(this LizardGraphics lizardGraphics) => lizardsGraphicsStorage.GetValue(lizardGraphics, _ => new LizardGraphicsData());
    }
}
