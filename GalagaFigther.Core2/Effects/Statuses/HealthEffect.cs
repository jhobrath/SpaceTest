using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class HealthEffect : PlayerEffect
    {
        protected override float Duration => 1f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.PlayerActions.Add(p => p.Health += 10); 
        }
    }
}
