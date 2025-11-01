using GalagaFighter.Core2.Effects;
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
    public class BarProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(0f, 1000f);
        private static Vector2 _baseSize => new(30f, 7f);

        public override float Damage => 3f;

        public BarProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
        }

        private static SpriteBase GetSprite()
        {
            return new AnimatedDrawnSprite(new(32,10), 12, .0625f, BarProjectileSpriteGenerator.CreateAnimatedBarProjectile);
        }

        public override List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
