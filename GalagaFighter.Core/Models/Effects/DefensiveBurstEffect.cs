using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace GalagaFighter.Core.Models.Effects
{
    public class DefensiveBurstEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/defensive_duck.png";
        protected override float Duration => 1f;
        public override List<string> DecorationKeys => new List<string> { "RepulseShield" };

        private readonly SpriteDecoration _sprite;
        public DefensiveBurstEffect()
        {
            _sprite = new SpriteDecoration(
                SpriteGenerationService.CreateAnimatedMagnetShieldSprite()
            )
            {
                Offset = new System.Numerics.Vector2(0, -80),
                Size = new System.Numerics.Vector2(200, 20)
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.IsRepulsive = true;
            modifiers.Decorations!["RepulseShield"] = _sprite;
        }
    }
}
