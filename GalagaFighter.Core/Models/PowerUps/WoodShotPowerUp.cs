using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Projectiles;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.PowerUps
{
    public class WoodShotPowerUp : PowerUp
    {
        public WoodShotPowerUp(IPowerUpController updater, Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
           : base(updater, owner, "Sprites/PowerUps/wood.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [new WoodShotEffect(player.PalleteSwap)];
        }
    }
}
