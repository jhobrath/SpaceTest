using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class DamageEffect : PlayerEffect
    {
        protected override float Duration => 1000f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Stats.DamageMultiplier *= 1.15f; 
        }
    }
}
