using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.PowerUps
{
    public class PowerUpCollectMovementBehavior : IPowerUpMovementBehavior
    {
        private Projectile _projectile;

        public PowerUpCollectMovementBehavior(Projectile projectile)
        {
            _projectile = projectile;
        }

        private const float _rotationChange = 100f;
        public void Apply(PowerUp powerUp)
        {
            var frameTime = Raylib.GetFrameTime();

            powerUp.Move(powerUp.Speed.X * frameTime, powerUp.Speed.Y * frameTime);
            powerUp.Rotation += frameTime * _rotationChange;
        }
    }
}
