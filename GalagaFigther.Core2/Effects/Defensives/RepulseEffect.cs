using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Defensives
{
    public class RepulseEffect : PlayerEffect
    {
        private readonly SpriteDecoration _sprite;

        public RepulseEffect()
        {
            _sprite = new SpriteDecoration(
                new AnimatedDrawnSprite(new(300, 35), 24, .015f, RepulseShieldSpriteGenerator.CreateAnimatedMagnetShieldSprite),
                new Vector2(90, 0),
                new Vector2(300, 35))
            { 
                CollectedFrom = Id,
                MaintainRotation = true
            };
        }

        protected override float Duration => .25f;

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Polarity -= 1f;
            modifiers.Decorations.Create[this] = (p,m) => [_sprite];
            modifiers.Stats.SpeedMultiplier *= 0f;
        }
    }
}
