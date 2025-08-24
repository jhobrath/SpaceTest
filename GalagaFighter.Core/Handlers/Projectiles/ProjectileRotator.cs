using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            projectile.Modifiers.RotationOffsetIncrement += ((projectile.Modifiers.RotationOffsetMultiplier * projectile.Modifiers.RotationOffsetIncrement) - projectile.Modifiers.RotationOffsetIncrement) * frameTime;
            projectile.Modifiers.RotationOffset += projectile.Modifiers.RotationOffsetIncrement * frameTime;
            projectile.Rotation = (projectile.Speed.X < 0 ? -180f : 0f) + projectile.Modifiers.RotationOffset;
        }
    }
}
