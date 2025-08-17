using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class NormalProjectile : Projectile
    {
        public static Vector2 BaseSize => new(30f, 15f);
        public static Vector2 BaseSpeed => new(1020f, 0f);

        public override int Damage => 5;

        public NormalProjectile(Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(GetSprite(initialSize), initialPosition, initialSize, initialSpeed)
        {
        }

        private static SpriteWrapper GetSprite(Vector2 initialSize)
        {
            return new SpriteWrapper(SpriteGenerator.CreateProjectileSprite(ProjectileType.Normal, (int)initialSize.X, (int)initialSize.Y));
        }
    }
}