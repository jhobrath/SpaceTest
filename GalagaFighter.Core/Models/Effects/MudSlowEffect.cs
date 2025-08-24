using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects
{
    public class MudSlowEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/mudsplaticon.png";
        public override int MaxCount => 3;        // Only one at a time (most recent)
        protected override float Duration => 0.2f; // Very short lifetime - continuously reapplied

        public MudSlowEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.SpeedMultiplier = 0.5f; // Slow the target player
            modifiers.Display.GreenAlpha = 0.7f;    // Green visual tint
        }
    }
}