using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class ElectricEffect : PlayerEffect
    {
        private EffectModifiers? _modifiers;

        public override string IconPath => "Sprites/Effects/burning.png";
        public override int MaxCount => 2;
        protected override float Duration => 1f;

        public ElectricEffect()
        {
           // AudioService.PlayBurningSound();
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.SpeedMultiplier = 0f;
            modifiers.Display.RedAlpha = .9f;
            modifiers.Display.GreenAlpha = .8f;
            modifiers.Display.BlueAlpha = .9f;
            _modifiers = modifiers;
        }
    }
}
