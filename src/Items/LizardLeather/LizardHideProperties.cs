using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Properties;

namespace SlugCrafting.Items
{
    public sealed class LizardHideProperties : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
            => throwable = true;

        public override void ScavCollectScore(Scavenger scavenger, ref int score)
            => score = 2;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
            => grabability = Player.ObjectGrabability.OneHand;
    }
}
