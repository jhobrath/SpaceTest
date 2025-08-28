using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.PowerUps
{
    public class FireRatePowerUp : PowerUp
    {
        private readonly List<PlayerEffect> _effects = [];

        public FireRatePowerUp(IPowerUpController controller, Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) 
            : base(controller, owner, "Sprites/PowerUps/firerate.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [new FireRateEffect()];
        }
    }
}
