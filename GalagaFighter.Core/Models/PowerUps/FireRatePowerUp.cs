using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.PowerUps
{
    public class FireRatePowerUp : PowerUp
    {
        public FireRatePowerUp(Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) 
            : base("Sprites/PowerUps/firerate.png", initialPosition, initialSize, initialSpeed)
        {
        }
    }
}
