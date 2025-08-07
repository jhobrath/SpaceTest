using Raylib_cs;

namespace GalagaFighter.Models
{
    public class IceProjectile : Projectile
    {
        public IceProjectile(Rectangle rect, float speed, Player owner) 
            : base(rect, speed, owner) 
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Ice, (int)rect.Width, (int)rect.Height);
        }

        public override int Damage => 0; // Ice projectiles don't do damage

        public override void OnHit(Player target, Game game)
        {
            target.SlowTimer = 5.0f; // Apply slow effect
            Owner.IceShotTimer = 0;  // Remove ice shot from owner
            game.PlayIceHitSound();
        }

        public override Color GetColor()
        {
            return Color.SkyBlue;
        }
    }
}