using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects
{
    public class FrozenEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/frozen.png";
        protected override float Duration => 5f; // Lasts 5 seconds

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.SpeedMultiplier *= .667f;
            modifiers.Stats.Shield *= 1.5f;
            modifiers.Stats.FireRateMultiplier *= 1.25f;
            modifiers.Display.BlueAlpha *= .4f;
        }
    }
}
