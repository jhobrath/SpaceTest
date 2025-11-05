using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public class DefaultShootEffect : PlayerEffect
    {
        protected override float Duration => 0f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.PlayerActions.Add(player =>
            {
                // Ensure at least one DefaultGun exists
                if (!player.Guns.Any(g => g is DefaultGun))
                    player.Guns.Add(new DefaultGun(player, false));
            });
        }
    }
}
