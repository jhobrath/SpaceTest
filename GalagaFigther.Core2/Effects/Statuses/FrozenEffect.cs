using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using System.Numerics;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class FrozenEffect : PlayerEffect
    {
        private SpriteDecoration _frozenDecoration;

        public FrozenEffect()
        {
            _frozenDecoration = new SpriteDecoration(
                new StillImageSprite("Sprites/Ships/MainShipBody_Frozen.png"), 
                Vector2.Zero, 
                false);
        }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Display.BlueAlpha *= 1.25f; 
            modifiers.Stats.SpeedMultiplier *= 0.5f; 
            modifiers.Stats.FireRate *= 1.25f;
            modifiers.Decorations[this] = _frozenDecoration;
        }
    }
}
