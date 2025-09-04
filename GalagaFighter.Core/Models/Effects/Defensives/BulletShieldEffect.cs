using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Defensives
{
    public class BulletShieldEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/effects/rewind.png";
        protected override float Duration => .35f;
        public override List<string> DecorationKeys => ["BulletShield"];

        private readonly SpriteDecoration _sprite;
        public BulletShieldEffect()
        {
            _sprite = new SpriteDecoration(SpriteGenerationService2.CreateBulletShieldEffect())
            {
                Offset = new System.Numerics.Vector2(0, -80),
                Size = new System.Numerics.Vector2(200, 20)
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.BulletShield = true;
            modifiers.Decorations!["BulletShield"] = _sprite;
        }
    }
}