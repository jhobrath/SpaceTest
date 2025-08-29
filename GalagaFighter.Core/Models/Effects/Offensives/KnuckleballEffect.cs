using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class KnuckleballEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/knuckleball.png";
        protected override float Duration => 10f;
        public KnuckleballEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.FireRateMultiplier = .75f;
            modifiers.Projectile.SpeedMultiplier *= 1.25f;
            modifiers.Projectile.Phases.Add(this, Roll());
            modifiers.Projectile.OnPhaseChange.Add(this, HandlePhaseChange);
            modifiers.Projectile.OnClone = (projMods) =>
            {
                projMods.Phases[this] = Roll();
                projMods.RotationOffsetIncrement = (float)Game.Random.NextDouble() * 150f;
            };
        }

        private List<float> Roll()
        {
            return [.. Enumerable.Range(1, 10).Select(x => (float)Game.Random.NextDouble() * 1.5f).OrderBy(x => x)];
        }

        private void HandlePhaseChange(Projectile projectile, int phase)
        {
            //Phase often, but exit most of the time
            //Otherwise each bullet would knuckle the same way at the same time
            if (Game.Random.NextDouble() > .5f)
                return;

            var ySpeeds = new List<float>([0f, 15f, 30f, -15f, -30f]);
            projectile.HurryTo(y: ySpeeds[Game.Random.Next(0,5)]*10f);
        }
    }
}
