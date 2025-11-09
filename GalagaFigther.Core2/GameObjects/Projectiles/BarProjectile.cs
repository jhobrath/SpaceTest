using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Services.Sprites;
using System.Numerics;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class BarProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(0f, 1000f);
        private static Vector2 _baseSize => new(30f, 7f);

        public BarProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Acceleration = new(500f,0);
            Damage = 3f;
            Behaviors.Add(typeof(RotationFollowsSpeedBehavior));
        }

        private static SpriteBase GetSprite()
        {
            return new AnimatedDrawnSprite(new(32,10), 12, .0625f, BarProjectileSpriteGenerator.CreateAnimatedBarProjectile);
        }

        public override List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
