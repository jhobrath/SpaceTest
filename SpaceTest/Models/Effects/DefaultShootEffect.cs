using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class DefaultShootEffect : ProjectileEffect
    {
        public DefaultShootEffect(Player player) : base(player) { }
        public override string IconPath => throw new NotImplementedException();
        protected override int ProjectileWidth => 30;
        protected override int ProjectileHeight => 15;
        protected override bool TotalUses => false;
        protected override float Duration => float.MaxValue;
        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new NormalProjectile(rect, speed, Player, this);
    }
}