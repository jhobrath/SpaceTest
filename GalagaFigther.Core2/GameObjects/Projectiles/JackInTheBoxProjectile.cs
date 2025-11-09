using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class JackInTheBoxProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(1500, 0f);
        private static Vector2 _baseSize => new(25, 50);

        public JackInTheBoxProjectile(Guid owner)
           : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Damage = 0;
            AngularVelocity = 1000f;
            Behaviors.Add(typeof(JackInTheBoxBehavior));
            EdgeCollisionHandler = typeof(JackInTheBoxEdgeCollisionBehavior);
        }

        private static SpriteBase GetSprite()
        {
            return new StillImageSprite("Sprites/Projectiles/JackInTheBox_clamped.png");
        }

        private static SpriteBase GetPopSprite()
        {
            return new NonRepeatingAnimatedImageSprite("Sprites/Projectiles/JackInTheBox_clamped.png", 20, 500, 50, .35f / 5f);
        }
    }
}
