using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class MudSlowEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/mudsplaticon.png";
        public override int MaxCount => 3;
        protected override float Duration => 0.2f;

        public MudSlowEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.SpeedMultiplier = 0.5f;
            modifiers.Display.GreenAlpha = 0.7f;
        }
    }
}