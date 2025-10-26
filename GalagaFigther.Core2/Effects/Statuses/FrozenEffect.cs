using GalagaFighter.Core2.Models.Players;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class FrozenEffect : PlayerEffect
    {

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Display.BlueAlpha *= 1.25f; 
            modifiers.Stats.SpeedMultiplier *= 0.5f; 
            modifiers.Stats.FireRate *= 1.25f;
        }
    }
}
