using RWCustom;
using UnityEngine;

using Fisobs.Properties;

namespace SlugCrafting.Items;

public sealed class StringTiedItemProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 1;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        => grabability = Player.ObjectGrabability.OneHand;
}