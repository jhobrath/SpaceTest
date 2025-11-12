using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public class PowerShotEffect : PlayerEffect
    {
        public override bool DeactivateAfterApply => true;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.PlayerActions.Add(p =>
            {
                p.Guns.Add(new PowerShotGun(p));
            });
        }
    }
}
