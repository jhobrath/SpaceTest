using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Models.Effects
{
    public class KnuckleballEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/knuckleball.png";
        public KnuckleballEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
               modifiers.Projectile.Phases.Add(this, Roll());
               modifiers.Projectile.OnPhaseChange = HandlePhaseChange;
               modifiers.Projectile.OnClone = () => Roll();
        }

        private List<float> Roll()
        {
            return [.. Enumerable.Range(1, 40).Select(x => x * (float)Game.Random.NextDouble() / 20f).OrderBy(x => x)];
        }

        private void HandlePhaseChange(Projectile projectile, PlayerEffect playerEffect, int phase)
        {
            if (playerEffect != this)
                return;

            //Phase often, but exit most of the time
            //Otherwise each bullet would knuckle the same way at the same time
            if (Game.Random.NextDouble() > .2f)
                return;

            var ySpeeds = new List<float>([0f, 10f, 20f, -10f, -20f]);
            projectile.HurryTo(y: ySpeeds[Game.Random.Next(0,5)]*10f);
        }
    }
}
