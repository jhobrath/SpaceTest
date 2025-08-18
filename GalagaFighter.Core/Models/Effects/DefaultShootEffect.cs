using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class DefaultShootEffect : PlayerEffect
    {
        public override bool IsProjectile => true;
        public SpriteWrapper _sprite;

        public DefaultShootEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/Player1.png"));
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Sprite = _sprite;
        }

    }
}
