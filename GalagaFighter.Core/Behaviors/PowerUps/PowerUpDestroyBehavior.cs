using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.PowerUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.PowerUps
{
    public class PowerUpDestroyBehavior : IPowerUpDestroyBehavior
    {
        public void Apply(PowerUp powerUp)
        {
            if (powerUp.Rect.Y > Game.Height)
                powerUp.IsActive = false;
        }
    }
}
