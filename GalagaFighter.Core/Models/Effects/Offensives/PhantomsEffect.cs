using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class PhantomsEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/phantoms.png";
        protected override float Duration => 20f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.PhantomCount += 4;
            base.Apply(modifiers);
        }
    }
}
