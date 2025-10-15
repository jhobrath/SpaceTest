using GalagaFigther.Core2.Helpers;
using GalagaFigther.Core2.Services.Sprites;
using System.Numerics;

namespace GalagaFigther.Core2.GameObjects.Projectiles
{
    public class DefaultProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(1000f, 0f);
        private static Vector2 _baseSize => new(50f, 10f);

        public DefaultProjectile(Guid owner, Vector2 position)
            : base(owner, position, _baseSize, GetSpeed(_baseSpeed), GetSprite())
        {
        }

        private static Vector2 GetSpeed(Vector2 baseSpeed)
        {
            return new Vector2(
                _baseSpeed.X,
                _baseSpeed.Y);
        }

        public override Vector2 GetBaseSpeed()
        {
            return _baseSpeed;
        }

        public override Vector2 GetBaseSize()
        {
            return _baseSize;
        }

        private static SpriteBase GetSprite()
        {
            return new DrawnSprite(DefaultProjectileSpriteGenerator.CreateProjectileSprite());
        }
    }
}
