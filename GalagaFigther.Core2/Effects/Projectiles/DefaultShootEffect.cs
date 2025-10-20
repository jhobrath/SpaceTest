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

        public override List<Gun> LeftGuns =>
        [
            new Gun([new GunBarrel(new(0, -46),new(30,-46))], HandleShoot)
        ];
        
        public override List<Gun> RightGuns =>
        [
            new Gun([new GunBarrel(new(0, 46),new(30,46))], HandleShoot)
        ];

        private readonly StillImageSprite _sprite;

        public DefaultShootEffect()
        {
            _sprite = new StillImageSprite("Sprites/Ships/MainShipGuns.png");
        }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Decorations.Add(new SpriteDecoration(_sprite) { MaintainAlpha = true });
            modifiers.LeftGuns.Add(nameof(DefaultShootEffect), LeftGuns);
            modifiers.RightGuns.Add(nameof(DefaultShootEffect), RightGuns);
        }

        private List<GameObject> HandleShoot(Guid owner)
        {
            return [new DefaultProjectile(owner, Vector2.Zero)];
        }
    }
}
