using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileRotator
    {
        void Rotate(Projectile projectile);
    }

    public class ProjectileRotator : IProjectileRotator
    {
        public void Rotate(Projectile projectile)
        {
            projectile.Rotation += projectile.Modifiers.RotationOffset;
            projectile.Rotation *= projectile.Modifiers.RotationMultiplier;
        }
    }
}
