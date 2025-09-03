using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Defensives
{
    public class ParryEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/rewind.png";
        protected override float Duration => .15f;
        public override List<string> DecorationKeys => ["ParryShield"];

        private readonly SpriteDecoration _sprite;
        public ParryEffect()
        {
            _sprite = new SpriteDecoration(SpriteGenerationService2.CreateAnimatedParrySprite())
            {
                Offset = new System.Numerics.Vector2(0, -80),
                Size = new System.Numerics.Vector2(200, 20)
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Parry = true;
            modifiers.Decorations!["ParryShield"] = _sprite;
        }
    }
}