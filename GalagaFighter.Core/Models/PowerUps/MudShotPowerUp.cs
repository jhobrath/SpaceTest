using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Projectiles;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.PowerUps
{
    public class MudShotPowerUp : PowerUp
    {
        public MudShotPowerUp(IPowerUpController controller, Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(controller, owner, "Sprites/PowerUps/mud.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return new List<PlayerEffect>
            {
                new MudShotEffect()
            };
        }
    }
}