using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class HookBiteEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/hookbite.png";
        protected override float Duration => 1000f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnNearProjectile.Add(HandleNearbyProjectile);
        }

        private void HandleNearbyProjectile(Projectile current, Projectile target)
        {
            var yDistance = Math.Abs(current.Center.Y - target.Center.Y);
            if (yDistance > 200)
                return;

            var hookPct = (200 - yDistance)/200;
            current.Modifiers.Homing = 10f*hookPct;
            current.Modifiers.OnNearProjectile = null;
        }
    }
}
