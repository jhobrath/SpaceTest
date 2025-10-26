using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class FrozenEffect : PlayerEffect
    {
        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Display.BlueAlpha *= .3f;
            modifiers.Stats.SpeedMultiplier *= .5f;
        }
    }
}
