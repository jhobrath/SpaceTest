using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Projectiles;
using GalagaFighter.Core2.Effects.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.PowerUps
{
    public class NinjaTurretPowerUp : PowerUp
    {
        public NinjaTurretPowerUp(Vector2 initialPosition, Vector2 initialSpeed) 
            : base(initialPosition, initialSpeed, "Sprites/PowerUps/addWeapon.png")
        {
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [new AddWeaponEffect()];
        }
    }
}
