using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class DoubleShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/doubleshot.png";
        protected override float Duration => 10f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.DoubleShot = true;
        }
    }
}
