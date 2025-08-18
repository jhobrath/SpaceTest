using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles
{
    public class IceCollisionBehavior : ProjectileCollisionBehavior
    {
        public IceCollisionBehavior(IObjectService objectService) : base(objectService)
        {
        }

        protected override DefaultCollision Spawn(Projectile projectile, Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity)
        {
            return new IceShotCollision(projectile.Id, initialPosition, initialSize, initialVelocity);
        }
    }
}
