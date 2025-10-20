using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Services;


namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public interface IProjectileShooter
    {
        void Shoot(GameObject gun, GameObject projectile, Vector2 gunTipOffset, Vector2? speedMultiplier = null);
    }

    public class ProjectileShooter : IProjectileShooter
    {
        private readonly IObjectService _objectService;

        public ProjectileShooter(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Shoot(GameObject gun, GameObject projectile, Vector2 gunTipOffset, Vector2? speedMultiplier = null)
        {
            var offsetRotationRadians = (gun.Rotation - 90) * MathF.PI / 180f;
            var gunTip = new Vector2(
                gun.Center.X + (gunTipOffset.X * MathF.Cos(offsetRotationRadians) - gunTipOffset.Y * MathF.Sin(offsetRotationRadians)),
                gun.Center.Y + (gunTipOffset.X * MathF.Sin(offsetRotationRadians) + gunTipOffset.Y * MathF.Cos(offsetRotationRadians))
            );

            MoveInPlace(projectile, gunTip, gun.Rotation, speedMultiplier);
            _objectService.Add(projectile);
        }

        private static void MoveInPlace(GameObject projectile, Vector2 spawnPoint, float parentRotation, Vector2? speedMultiplier = null)
        {
            speedMultiplier ??= new(1, 1);
            var speedLength = projectile.Speed.Length();

            var gunRotationRadians = (90 - parentRotation) * MathF.PI / 180f;
            var speedXPct = MathF.Cos(gunRotationRadians);
            var speedYPct = -MathF.Sin(gunRotationRadians);

            projectile.HurryTo(speedLength * speedXPct * speedMultiplier.Value.X, speedLength * speedYPct * speedMultiplier.Value.Y);

            var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var rotation = theta * 180.0f / MathF.PI;
            projectile.Rotation = rotation;

            var halfWidth = projectile.Width / 2f;
            var halfHeight = projectile.Height / 2f;

            projectile.MoveTo(spawnPoint.X - halfWidth, spawnPoint.Y - halfHeight);
        }
    }
}
