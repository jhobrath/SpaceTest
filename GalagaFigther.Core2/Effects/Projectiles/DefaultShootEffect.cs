using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.GameObjects.Projectiles;
using GalagaFigther.Core2.Helpers;
using GalagaFigther.Core2.Models;
using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            modifiers.Decorations.Add(new SpriteDecoration(_sprite) { MaintainAlpha = true });
            modifiers.Projectile.OnShoot[nameof(DefaultShootEffect)] = HandleShoot;
        }

        private List<GameObject> HandleShoot(Guid owner, Vector2 position)
        {
            return [new DefaultProjectile(owner, position)];
        }
    }
}
