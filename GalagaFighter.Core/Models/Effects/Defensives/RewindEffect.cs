using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Defensives
{
    public class RewindEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/rewind.png";
        protected override float Duration => 1f;
        public override List<string> DecorationKeys => ["RepulseShield"];

        private readonly SpriteDecoration _sprite;
        public RewindEffect()
        {
            _sprite = new SpriteDecoration(SpriteGenerationService.CreateAnimatedMagnetShieldSprite())
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