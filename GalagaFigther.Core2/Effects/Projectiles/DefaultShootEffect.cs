using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public class DefaultShootEffect : ProjectileEffect
    {
        protected override float Duration => 0f;

        public override Vector2 GunOffset => new(30, -46);

        private readonly StillImageSprite _sprite;

        public DefaultShootEffect()
        {
            _sprite = new StillImageSprite("Sprites/Ships/MainShipGuns.png");
        }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Decorations.Add(new SpriteDecoration(_sprite) { MaintainAlpha = true });
            modifiers.Projectile.OnShoot[nameof(DefaultShootEffect)] = HandleShoot;
            base.Apply(modifiers);
        }

        private List<GameObject> HandleShoot(Guid owner)
        {
            return [new DefaultProjectile(owner, Vector2.Zero)];
        }
    }
}
