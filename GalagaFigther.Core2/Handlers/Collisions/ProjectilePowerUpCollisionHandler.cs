using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.PowerUps;
using GalagaFighter.Core2.Services;
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
        void Handle(Projectile projectile, PowerUp powerUp);
    }
    public class ProjectilePowerUpCollisionHandler : IProjectilePowerUpCollisionHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public ProjectilePowerUpCollisionHandler(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Handle(Projectile projectile, PowerUp powerUp)
        {
            if(powerUp.Owner != Game.Id)
            {
                var collectionData = _gameDataRegistry.Get<PowerUpCollectionData>(powerUp);
                if (collectionData.SinceHit > .75f)
                    return;
            }

            powerUp.Owner = projectile.Owner;
        }
    }
}
