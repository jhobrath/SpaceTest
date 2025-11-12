using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Services.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class PowerShotProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(0f, 1000f);
        private static Vector2 _baseSize => new(50f, 200f);

        public PowerShotProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Acceleration = new(500f, 0);
            Damage = 3f;
            Behaviors.Add(typeof(RotationFollowsSpeedBehavior));
            Behaviors.Add(typeof(DamageByDistanceBehavior));
        }

        private static SpriteBase GetSprite()
        {
            return new AnimatedDrawnSprite(new(50f, 200f), 12, .0625f, PowerShotSpriteGenerator.GeneratePowerShotSprite);
        }
    }
}
