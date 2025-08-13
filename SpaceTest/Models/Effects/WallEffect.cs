using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class WallEffect : ProjectileEffect
    {
        public WallEffect(Player player) : base(player) { }

        protected override int ProjectileWidth => 150;
        protected override int ProjectileHeight => 15;
        protected override bool OneTimeUse => true;
        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new WallProjectile(rect, speed, Player, this);
        protected override string Texture => "Sprites/Players/WoodShotShip.png";
    }
}