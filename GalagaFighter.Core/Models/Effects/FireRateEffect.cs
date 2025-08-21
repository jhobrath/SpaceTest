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
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.FireRateMultiplier *= .75f;
        }
    }
}
