using Raylib_cs;
using GalagaFighter.Models.Players;

namespace GalagaFighter.Models
{
    public class NormalProjectile : Projectile
    {
        public NormalProjectile(Rectangle rect, float speed, Player owner)
            : base(rect, speed, owner)
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Normal, (int)rect.Width, (int)rect.Height);
        }

        public override int Damage => 20;

        public override void OnHit(Player target, Game game)
        {
            target.TakeDamage(Damage);
            game.PlayHitSound();
        }

        public override Color GetColor()
        {
            return Color.Red;
        }
    }
}