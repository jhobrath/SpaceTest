using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Projectiles;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.PowerUps
{
    public class MagnetPowerUp : PowerUp
    {
        public MagnetPowerUp(IPowerUpController updater, Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
           : base(updater, owner, "Sprites/PowerUps/magnet.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [new MagnetEffect()];
        }
    }
}
