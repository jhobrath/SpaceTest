using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class IceShotEffect : ProjectileEffect
    {
        protected override float Duration => 10.0f;

        public IceShotEffect(Player player) : base(player)
        {
        }

        protected override int ProjectileWidth => 95;
        protected override int ProjectileHeight => 42;
        protected override bool OneTimeUse => false;
        protected override float? OnHitMaxRemainingTime => 2;
        protected override string Texture => "Sprites/Players/IceShotShip.png";

        protected override Vector2 SpawnOffset => new Vector2(-50, 0);

        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new IceProjectile(rect, speed, Player, this);

    }
}