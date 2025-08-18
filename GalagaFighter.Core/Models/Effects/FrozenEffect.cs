using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class FrozenEffect : PlayerEffect
    {
        public override void Apply(PlayerStats stats)
        {
            stats.MovementSpeed *= .667f;
            stats.Shield *= 1.5f;
            stats.FireRate *= 1.25f;
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Color = display.Color.ApplyBlue(.4f);
        }
    }
}
