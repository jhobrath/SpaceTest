using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class IceShotShootingBehavior : PlayerShootingBehavior, IPlayerShootingBehavior
    {
        public IceShotShootingBehavior(IObjectService objectService) 
            : base(objectService)
        {
        }

        protected override Vector2 GetSpawnSize(Player player)
        {
            return IceProjectile.BaseSize;
        }

        protected override Projectile Create(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {

            var projectile = new IceProjectile(owner, initialPosition, initialSize, initialSpeed);
            projectile.SetMovementBehavior(new ProjectileMovementBehavior());
            projectile.SetDestroyBehavior(new ProjectileDestroyBehavior());
            projectile.SetCollisionBehavior(new IceShotCollisionBehavior(_objectService));

            if (projectile.Speed.X < 0)
                projectile.Rotation = -180f;

            _objectService.AddGameObject(projectile);

            return projectile;
        }
    }
}
