using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using Raylib_cs;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class NormalProjectile : Projectile
    {
        public NormalProjectile(Rectangle rect, Vector2 speed, Player owner)
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