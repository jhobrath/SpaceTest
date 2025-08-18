using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.PowerUps
{
    public class PowerUpCollisionBehavior : IPowerUpCollisionBehavior
    {
        private readonly IObjectService _objectService;

        public PowerUpCollisionBehavior(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Apply(PowerUp powerUp, Projectile projectile)
        {
            var player = _objectService.GetOwner(projectile);
            powerUp.SetOwner(player.Id);

            powerUp.SetMovementBehavior(new PowerUpCollectMovementBehavior(_objectService, powerUp, player));
            powerUp.SetDestroyBehavior(new PowerUpCollectDestroyBehavior(_objectService, projectile));

            //var collisionSize = new Vector2(20f, 20f);
            //var xOffset = projectile.Speed.X < 0 ? 0 : projectile.Rect.Width;
            //var position = new Vector2(powerUp.Center.X - collisionSize.X / 2 + xOffset, powerUp.Center.Y - collisionSize.Y / 2);
            //var collision = new SmallCollision(powerUp.Id, position, collisionSize, projectile.Speed);
            //_objectService.AddGameObject(collision);
        }
    }
}
