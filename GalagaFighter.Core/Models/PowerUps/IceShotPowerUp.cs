using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.PowerUps
{
    public class IceShotPowerUp : PowerUp
    {
        public IceShotPowerUp(IPowerUpController updater, Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) 
            : base(updater, owner, "Sprites/PowerUps/ice.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [
                new IceShotEffect()
            ];
        }
    }
}
