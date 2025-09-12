using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Defensives
{
    public class WrapEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/rewind.png";
        protected override float Duration => 1.5f;


        public WrapEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Wrap = true;
        }
    }
}