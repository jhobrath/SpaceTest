using GalagaFighter.Core2.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.PowerUps
{
    public class FireRatePowerUp : PowerUp
    {
        public FireRatePowerUp(Vector2 initialPosition, Vector2 initialSpeed) 
            : base(initialPosition, initialSpeed, "Sprites/PowerUps/firerate.png")
        {
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
