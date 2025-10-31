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
    public class ShotGunShellProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(.0000001f, 0f);
        private static Vector2 _baseSize => new(600f, 100f);

        public override float Damage => .5f;

        public ShotGunShellProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Lifetime = 5f;
        }

        private static SpriteBase GetSprite()
        {
            return new AnimatedDrawnSprite(new(600f,100f), 12, 1f, ShotGunShellProjectileSpriteGenerator.CreateAnimatedShotGunShellProjectile);
        }

        public override List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
