using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
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
            if(projectile.Modifiers.RotationOffset != 0)
                projectile.Rotation += projectile.Modifiers.RotationOffset * frameTime;

            if(projectile.Modifiers.RotationMultiplier != 0)
                projectile.Rotation += (projectile.Rotation * projectile.Modifiers.RotationMultiplier - projectile.Rotation)*frameTime;
        }
    }
}
