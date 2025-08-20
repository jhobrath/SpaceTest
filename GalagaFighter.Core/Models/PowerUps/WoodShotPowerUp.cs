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
    public class WoodShotPowerUp : PowerUp
    {
        public WoodShotPowerUp(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
           : base(owner, "Sprites/PowerUps/wood.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects(IEventService eventService, IObjectService objectService, IInputService inputService)
        {
            var effect = new WoodShotEffect(eventService, objectService, inputService);
            effect.SetOwner(this.Owner); // Track which player owns the effect
            return new List<PlayerEffect> { effect };
        }
    }
}
