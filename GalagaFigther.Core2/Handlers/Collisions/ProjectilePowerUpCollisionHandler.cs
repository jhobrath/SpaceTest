using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Collisions
{
    public interface IProjectilePowerUpCollisionHandler
    {
        void Handle(Projectile projectile, PowerUp powerUp, Vector2 point);
    }
    public class ProjectilePowerUpCollisionHandler : IProjectilePowerUpCollisionHandler
    {
        public void Handle(Projectile projectile, PowerUp powerUp, Vector2 point)
        {
            powerUp.Owner = projectile.Owner;
        }
    }
}
