using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;

namespace GalagaFighter.Core.Models.Effects
{
    public class HookBiteEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/hookbite.png";
        protected override float Duration => 10f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnNearProjectile = HandleNearbyProjectile;
        }

        private void HandleNearbyProjectile(Projectile projectile, Projectile target, Player player, Player targetPlayer)
        {
            var projectileDistanceY = target.Center.Y - projectile.Center.Y;
            var playerDistanceY = targetPlayer.Center.Y - projectile.Center.Y;
            var playerDistanceX = Math.Abs(targetPlayer.Center.X - projectile.Center.X);
            
            //Too overpoweredd
            if (playerDistanceX < 250)
                return;

            //Only hook if hooking takes the bullet towards the opponent
            if (projectileDistanceY < 0 && playerDistanceY > 0 ||
                projectileDistanceY > 0 && playerDistanceY < 0)
                return;

            var remainingTime = playerDistanceX / Math.Abs(projectile.Speed.X);
            //projectile.Modifiers.VerticalPositionOffset = playerDistanceY/remainingTime;
            projectile.Modifiers.VerticalPositionIncrement = 2 * playerDistanceY / (remainingTime * remainingTime);
            //projectile.Modifiers.VerticalPositionMultiplier = .5f;

            projectile.Modifiers.OnNearProjectile = null;
            target.IsActive = false;
        }
    }
}
