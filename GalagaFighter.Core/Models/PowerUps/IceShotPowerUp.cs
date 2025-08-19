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
        public IceShotPowerUp(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) 
            : base(owner, "Sprites/PowerUps/ice.png", initialPosition, initialSize, initialSpeed)
        {
        }

        public override List<PlayerEffect> CreateEffects(IEventService eventService,IObjectService objectService, IInputService inputService)
        {
            return [
                new IceShotEffect(objectService)
            ];
        }
    }
}
