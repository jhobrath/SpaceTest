using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class HomingEffect : OffensiveEffect
    {
        public override string IconPath => "Sprites/effects/offensives/homing.png";
        protected override float Duration => 10f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.Homing += 1f;
        }
    }
}
