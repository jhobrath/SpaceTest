using GalagaFigther.Core2.Helpers;
using GalagaFigther.Core2.Models;
using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Effects.Projectiles
{
    public class DefaultShootEffect : PlayerEffect
    {
        private readonly StillImageSprite _sprite;

        public DefaultShootEffect()
        {
            _sprite = new StillImageSprite("Sprites/Ships/MainShipGuns.png");
        }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Decorations.Add(new SpriteDecoration(_sprite, followRotation: false));
        }
    }
}
