using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class FireRateEffect : StatusEffect
    {
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.FireRateMultiplier *= .75f;
        }
    }
}
