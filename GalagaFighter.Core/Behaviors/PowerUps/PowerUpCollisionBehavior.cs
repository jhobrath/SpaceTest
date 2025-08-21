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
        }
    }
}
