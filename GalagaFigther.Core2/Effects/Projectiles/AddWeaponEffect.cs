using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public class AddWeaponEffect : PlayerEffect
    {
        protected override float Duration => 0f;
        public override bool DeactivateAfterApply => true;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.PlayerActions.Add(player =>
            {
                var defaultGun = player.Guns.OfType<DefaultGun>().FirstOrDefault();
                var hasBarGun = player.Guns.Any(g => g is BarGun);
                var hasHomingGun = player.Guns.Any(g => g is HomingGun);
                var hasShotGun = player.Guns.Any(g => g is ShotGun);

                if (!hasBarGun)
                    player.Guns.Add(new BarGun(player));
                else if (!hasHomingGun)
                    player.Guns.Add(new HomingGun(player));
                else if (defaultGun != null && !defaultGun.ShootBoth)
                    defaultGun.ShootBoth = true;
                else if (!hasShotGun)
                    player.Guns.Add(new ShotGun(player));
            });
        }
    }
}
