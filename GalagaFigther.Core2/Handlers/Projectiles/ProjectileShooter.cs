using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public interface IProjectileShooter
    {
        void Shoot(GameObject gun, GameObject projectile, GunBarrel barrel);
    }

    public class ProjectileShooter : IProjectileShooter
    {
        private readonly IObjectService _objectService;

        public ProjectileShooter(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Shoot(GameObject objectShooting, GameObject projectile, GunBarrel barrel)
        {
            var barrelStart = GetRotatedOffset(objectShooting, barrel.Start);
            var barrelEnd = GetRotatedOffset(objectShooting, barrel.End);

            MoveInPlace(objectShooting, projectile, barrelStart, barrelEnd);
            _objectService.Add(projectile);
        }

        private static void MoveInPlace(GameObject objectShooting, GameObject projectile, Vector2 barrelStart, Vector2 barrelEnd)
        {
            var barrelVector = barrelEnd - barrelStart; 
            var barrelRotationRadians = MathF.Atan2(-barrelVector.Y, barrelVector.X);

            var speedLength = projectile.Speed.Length();
            var speedXPct = MathF.Cos(barrelRotationRadians);
            var speedYPct = -MathF.Sin(barrelRotationRadians);

            projectile.HurryTo(speedLength * speedXPct, speedLength * speedYPct);

            var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var rotation = theta * 180.0f / MathF.PI;
            projectile.Rotation = rotation;

            var halfWidth = projectile.Width / 2f;
            var halfHeight = projectile.Height / 2f;

            projectile.MoveTo(barrelEnd.X - halfWidth, barrelEnd.Y - halfHeight);
        }

        private Vector2 GetRotatedOffset(GameObject objectShooting, Vector2 offset)
        {
            var offsetRotationRadians = (objectShooting.Rotation - 90) * MathF.PI / 180f;
            return objectShooting.Center + new Vector2(
                (offset.X * MathF.Cos(offsetRotationRadians) - offset.Y * MathF.Sin(offsetRotationRadians)),
                (offset.X * MathF.Sin(offsetRotationRadians) + offset.Y * MathF.Cos(offsetRotationRadians))
            );
        }
    }
}
