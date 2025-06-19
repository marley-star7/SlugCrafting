using CompartmentalizedCreatureGraphics;

namespace SlugCrafting.Items;

public class LizardShellScavenge : AbstractPhysicalObjectScavenge
{
    Lizard lizard;

    public LizardShellScavenge(Lizard lizard) : base(lizard)
    {
        this.lizard = lizard;
    }

    public override AbstractPhysicalObject Scavenge()
    {
        var lizardGraphics = lizard.graphicsModule as LizardGraphics;
        var sLeaser = lizardGraphics.GetGraphicsModuleCCGData().sLeaser;

        var shell = new AbstractLizardShell(lizard.room.world, lizard.Template.type, lizard.coord, lizard.room.game.GetNewID())
        {
            // Copy all the data of the lizard to the scavenged shell.
            shellColor = lizardGraphics.effectColor,
            headBodyChunkRadius = lizard.firstChunk.rad,

            //-- MR7 Reduce the mass of the head chunk to make it ACTUALLY carryable lol (green liz especially), dw the mass still matters.
            // However, the choice of lizard shells (green liz especially) still being so heavy was purposeful decision on my end,
            // Acting as a balance decision that getting to bring a shell to work with is harder the heavier,
            // often requiring baiting of the lizard or some other means, since green lizard armor for example is so good.
            headBodyChunkMass = lizard.firstChunk.mass * 0.3f,

            headSprite0Jaw = sLeaser.sprites[lizardGraphics.SpriteHeadStart].element.name,
            headSprite1LowerTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 1].element.name,
            headSprite2UpperTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 2].element.name,
            headSprite3Head = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 3].element.name,
            headSprite4Eyes = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 4].element.name,
        };
        canScavenge = false; // Prevents the scavenge from being used again.

        return shell;
    }
}