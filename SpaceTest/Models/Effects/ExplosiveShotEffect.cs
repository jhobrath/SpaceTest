using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class ExplosiveShotEffect : ProjectileEffect
    {
        protected override float Duration => 10.0f;

        public ExplosiveShotEffect(Player player) : base(player)
        {
        }

        protected override int ProjectileWidth => 60;
        protected override int ProjectileHeight => 40;
        protected override bool OneTimeUse => false;
        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new ExplosiveProjectile(rect, speed, Player, this);
        //protected override string Texture => "Sprites/Players/ExplosiveShotShip.png";

        protected override Vector2 SpawnOffset => new Vector2(-50, 15);
    }
}