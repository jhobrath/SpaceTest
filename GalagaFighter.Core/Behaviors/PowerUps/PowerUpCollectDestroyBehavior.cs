using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.PowerUps
{
    public class PowerUpCollectDestroyBehavior : IPowerUpDestroyBehavior
    {
        private readonly IObjectService _objectService;
        private readonly Player _player;

        public PowerUpCollectDestroyBehavior(IObjectService objectService, Projectile projectile)
        {
            _objectService = objectService;
            _player = (Player)_objectService.GetOwner(projectile);
        }

        public void Apply(PowerUp powerUp)
        {
            if (Math.Abs(_player.Center.X - powerUp.Center.X) < 50)
            {
                powerUp.IsActive = false;

                foreach (var effect in powerUp.Effects)
                    _player.AddEffect(effect);
            }
        }
    }
}
