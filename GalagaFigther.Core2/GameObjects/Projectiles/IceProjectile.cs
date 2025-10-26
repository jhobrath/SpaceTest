using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class IceProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(2020f, 0f);
        private static Vector2 _baseSize => new(95f, 42f);

        public IceProjectile(Guid owner) 
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
        }

        private static SpriteBase GetSprite()
        {
            return new NonRepeatingAnimatedImageSprite("Sprites/Projectiles/ice.png", 6, 570 / 6, 42, .125f);
        }
    }
}
