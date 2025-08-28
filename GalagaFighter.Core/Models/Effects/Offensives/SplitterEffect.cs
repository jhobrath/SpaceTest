using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class SplitterEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/splitter.png";
        protected override float Duration => 10f;
        public override bool IsProjectile => false;

        public SplitterEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.CanSplit = true;
        }
    }
}
