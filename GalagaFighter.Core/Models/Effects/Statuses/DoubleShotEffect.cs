using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class DoubleShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/doubleshot.png";
        protected override float Duration => 10f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.DoubleShot = true;
        }
    }
}
