using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.PowerUps
{
    public class HealthPowerUp : PowerUp
    {
        public HealthPowerUp(Vector2 initialPosition, Vector2 initialSpeed) 
            : base(initialPosition, initialSpeed, "Sprites/PowerUps/health.png")
        {
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [new HealthEffect()];
        }
    }
}
