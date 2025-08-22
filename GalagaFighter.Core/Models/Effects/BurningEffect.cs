using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects
{
    public class BurningEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/burning.png";
        public override int MaxCount => 5;
        protected override float Duration => 3f;

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.FireRateMultiplier *= .75f;
            modifiers.Display.RedAlpha *= .6f;
        }
    }
}
