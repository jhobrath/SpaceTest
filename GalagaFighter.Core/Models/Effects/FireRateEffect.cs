using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class FireRateEffect : PlayerEffect
    {
        public override void Apply(PlayerStats stats)
        {
            stats.FireRate *= .75f;
        }
    }
}
