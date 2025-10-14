using GalagaFigther.Core2.Helpers;
using GalagaFigther.Core2.Services.Sprites;
using System.Numerics;

namespace GalagaFigther.Core2.GameObjects.Projectiles
{
    public class DefaultProjectile : Projectile
    {
        public DefaultProjectile(Vector2 position)
            : base(position, GetSize(), GetSpeed(), GetSprite())
        {
        }

        private static Vector2 GetSpeed()
        {
            return new(1000f, 1000F);
        }

        private static Vector2 GetSize()
        {
            throw new NotImplementedException();
        }

        private static SpriteBase GetSprite()
        {
            return new DrawnSprite(DefaultProjectileSpriteGenerator.CreateProjectileSprite());
        }
    }
}
