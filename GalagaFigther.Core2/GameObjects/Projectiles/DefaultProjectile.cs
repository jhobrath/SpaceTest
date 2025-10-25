using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Services.Sprites;
using System.Numerics;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class DefaultProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(0f, 2000f);
        private static Vector2 _baseSize => new(30f, 10f);

        public DefaultProjectile(Guid owner, Vector2 position)
            : base(owner, position, _baseSize, _baseSpeed, GetSprite())
        {
        }

        private static SpriteBase GetSprite()
        {
            return new DrawnSprite((color) => 
                DefaultProjectileSpriteGenerator.CreateProjectileSprite(color: color));
        }
    }
}
