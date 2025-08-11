using GalagaFighter.Models.Players;
using SpaceTest.Models.Projectiles;
using System.Drawing;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class NinjaShotEffect : ProjectileEffect
    {
        protected override float Duration => 10.0f;

        public NinjaShotEffect(Player player) : base(player)
        {
        }

        protected override int ProjectileWidth => 60;
        protected override int ProjectileHeight => 40;
        protected override bool OneTimeUse => false;

    }
}