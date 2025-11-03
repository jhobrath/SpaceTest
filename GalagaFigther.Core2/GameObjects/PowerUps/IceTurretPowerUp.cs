using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.Effects.Statuses;
using GalagaFighter.Core2.Effects.Turrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.PowerUps
{
    public class IceTurretPowerUp : PowerUp
    {
        public IceTurretPowerUp(Vector2 initialPosition, Vector2 initialSpeed) 
            : base(initialPosition, initialSpeed, "Sprites/PowerUps/iceturret.png")
        {
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [new IceTurretEffect()];
        }
    }
}
