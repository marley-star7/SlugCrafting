using RWCustom;
using UnityEngine;

using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

public sealed class StringProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 1;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        => grabability = Player.ObjectGrabability.OneHand;
}
