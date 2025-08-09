using GalagaFighter.Models.Players;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class WallEffect : ProjectileEffect
    {
        private bool wasActive;
        public WallEffect(Player player) : base(player) { }

        protected override int ProjectileWidth => 150;
        protected override int ProjectileHeight => 15;
        protected override bool OneTimeUse => true;

        public override void OnActivate()
        {
        }

        public override void OnDeactivate()
        {
        }
    }
}