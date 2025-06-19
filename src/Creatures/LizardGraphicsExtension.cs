/*
using System.Runtime.CompilerServices;

namespace SlugCrafting.Creatures
{
    /// <summary>
    /// Saves mainly the sprite leaser for each lizard in the lizard's graphics,
    /// So we can reference later when stealing their heads via scavenging!
    /// </summary>
    public class LizardGraphicsCraftingData
    {
        public RoomCamera.SpriteLeaser sLeaser;
    }

    public static class LizardGraphicsExtension
    {
        private static readonly ConditionalWeakTable<LizardGraphics, LizardGraphicsCraftingData> lizardsGraphicsStorage = new();

        public static LizardGraphicsCraftingData GetLizardGraphicsCraftingData(this LizardGraphics lizardGraphics) => lizardsGraphicsStorage.GetValue(lizardGraphics, _ => new LizardGraphicsCraftingData());
    }
}
*/