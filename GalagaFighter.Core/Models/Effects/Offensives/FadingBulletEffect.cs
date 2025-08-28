using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class FadingBulletEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/fadingbullet.png";

        private List<float> _fadeLocations = [];
        protected override float Duration => 10f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnClone = (projMods) =>
            {
                projMods.Phases[this] = Roll();
                _fadeLocations = Enumerable.Range(1, 1).Select(x => 100 + (float)Game.Random.NextDouble() * (Game.Width - 200)).ToList();
            };
            modifiers.Projectile.OnPhaseChange = HandlePhaseChange;
        }

        private void HandlePhaseChange(Projectile projectile, PlayerEffect effect, int arg3)
        {
            var distanceForCalc = Math.Max(0, Math.Min(400, _fadeLocations.Min(x => Math.Abs(projectile.Center.X - x))) - 100);
            var alpha = distanceForCalc / 300f;
            projectile.Modifiers.Opacity = alpha;
        }

        private List<float> Roll()
        {
            return Enumerable.Range(1, 40).Select(x => x * (float)Game.Random.NextDouble() / 20f).OrderBy(x => x).ToList();
        }
    }
}
