using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects
{
    public class DefensiveDuckEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/defensive_duck.png";

        protected override float Duration => 1f;
        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Display.SizeMultiplier = new System.Numerics.Vector2(.85f, .85f);
            modifiers.Display.Opacity = .6f;
            modifiers.Untouchable = true;
        }
    }
}
