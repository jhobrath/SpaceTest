using GalagaFighter.Core.Models.Effects;
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
        private readonly List<PlayerEffect> _effects = [];
        public override List<PlayerEffect> Effects => _effects;

        public FireRatePowerUp(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) 
            : base(owner, "Sprites/PowerUps/firerate.png", initialPosition, initialSize, initialSpeed)
        {
            _effects.Add(new FireRateEffect());
        }
    }
}
