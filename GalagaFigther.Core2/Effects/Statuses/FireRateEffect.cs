using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class FireRateEffect : PlayerEffect
    {
        protected override float Duration => 1000f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.FireRate *= .85f;
        }
    }
}
