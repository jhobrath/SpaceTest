using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IProjectileRotator
    {
        IProjectileRotator Create();
        void Rotate(Projectile projectile);
    }

    public class ProjectileRotator : IProjectileRotator
    {
        public IProjectileRotator Create()
        {
            return new ProjectileRotator();
        }

        public void Rotate(Projectile projectile)
        {
            var frameTime = Raylib.GetFrameTime();

            if (projectile.Speed.X < .01f && projectile.Rotation != 0)
                return;

            projectile.Modifiers.RotationOffsetIncrement += ((projectile.Modifiers.RotationOffsetMultiplier * projectile.Modifiers.RotationOffsetIncrement) - projectile.Modifiers.RotationOffsetIncrement) * frameTime;
            projectile.Modifiers.RotationOffset += projectile.Modifiers.RotationOffsetIncrement * frameTime;
            var rotationBasedOnSpeed = MathF.Atan2(projectile.BaseSpeed.Y, projectile.BaseSpeed.X) * 180/MathF.PI;

            projectile.Rotation = rotationBasedOnSpeed + projectile.Modifiers.RotationOffset;
        }
    }
}
