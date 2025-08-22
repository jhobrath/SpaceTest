using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class MagnetEffect : PlayerEffect
    {
        private SpriteWrapper _sprite;

        public override string IconPath => "Sprites/Effects/Magnetshot.png";
        protected override float Duration => 5f;
        public override bool IsProjectile => true;


        public MagnetEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/MagnetShotShip.png"), 3, .33f);
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Magnetic = true;
            modifiers.Sprite = _sprite;
        }
    }
}
