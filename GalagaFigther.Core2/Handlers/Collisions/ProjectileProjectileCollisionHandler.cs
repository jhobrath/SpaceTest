using GalagaFighter.Core2.GameObjects.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Collisions
{
    public interface IProjectileProjectileCollisionHandler
    {
        void Handle(Projectile projectile1, Projectile projectile2);
    }
    public class ProjectileProjectileCollisionHandler : IProjectileProjectileCollisionHandler
    {
        public void Handle(Projectile projectile1, Projectile projectile2)
        {
            if (projectile1.Owner == projectile2.Owner)
                return;

            if (projectile2.DestroyProjectiles && !projectile1.DestroyProjectiles)
                projectile1.IsActive = false;

            if(projectile1.DestroyProjectiles && !projectile2.DestroyProjectiles)
                projectile2.IsActive = false;
        }
    }
}
