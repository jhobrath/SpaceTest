using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class NormalProjectile : Projectile
    {
        public NormalProjectile(Rectangle rect, Vector2 speed, Player owner, ProjectileEffect ownerEffect)
            : base(rect, speed, owner, ownerEffect)
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Normal, (int)rect.Width, (int)rect.Height);
        }

        public override int Damage => 5;

        public override Color GetColor()
        {
            return Color.Red;
        }
    }
}