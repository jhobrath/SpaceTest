using GalagaFigther.Core2.Helpers;
using GalagaFigther.Core2.Services.Sprites;
using System.Numerics;

namespace GalagaFigther.Core2.GameObjects.Projectiles
{
    public class DefaultProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(2000f, 0f);
        private static Vector2 _baseSize => new(30f, 10f);

        protected override Vector2 BaseSpeed => _baseSpeed;
        protected override Vector2 BaseSize => _baseSize;

        public DefaultProjectile(Guid owner, Vector2 position)
            : base(owner, position, _baseSize, _baseSpeed, GetSprite())
        {
        }

        private static SpriteBase GetSprite()
        {
            return new DrawnSprite(DefaultProjectileSpriteGenerator.CreateProjectileSprite());
        }
    }
}
