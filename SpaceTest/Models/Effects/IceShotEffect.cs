using GalagaFighter.Models.Players;
using SpaceTest.Models.Projectiles;
using System.Drawing;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class IceShotEffect : ProjectileEffect
    {
        private readonly float duration = 10.0f;
        private float remainingTime;
        private bool wasActive;

        public IceShotEffect(Player player) : base(player)
        {
            remainingTime = duration;
        }

        protected override int ProjectileWidth => 40;
        protected override int ProjectileHeight => 20;
        protected override bool OneTimeUse => false;

        public override void OnActivate()
        {
            wasActive = true;
        }

        public override void OnUpdate(float frameTime)
        {
            if (!wasActive) return;
            remainingTime -= frameTime;
            if (remainingTime <= 0)
            {
                IsActive = false;
            }
            base.OnUpdate(frameTime);
        }

        public override void OnDeactivate()
        {
        }
    }
}