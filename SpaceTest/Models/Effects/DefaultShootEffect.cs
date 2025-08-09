using GalagaFighter.Models.Players;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class DefaultShootEffect : ProjectileEffect
    {
        public DefaultShootEffect(Player player) : base(player) { }

        protected override int ProjectileWidth => 30;
        protected override int ProjectileHeight => 15;
        protected override bool OneTimeUse => false;
    }
}