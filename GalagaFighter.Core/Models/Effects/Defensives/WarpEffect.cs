using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Defensives
{
    public class WarpEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/rewind.png";
        protected override float Duration => .5f;


        public WarpEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Warp = true;
        }
    }
}