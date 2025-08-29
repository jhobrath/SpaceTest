using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class IceShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/iceshot.png";
        public override bool IsProjectile => true;
        private readonly SpriteWrapper _sprite;
        protected override float Duration => 10f;

        private SpriteDecorations _decorations = [];

        public IceShotEffect(Color? color)
        {
            _sprite = new SpriteWrapper("Sprites/Ships/MainShip.png", color ?? Color.White);
            _decorations = new SpriteDecorations()
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipIceGuns.png"))),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipIceGuns_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipIceGuns_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipIceGuns_ShootRight.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Decorations = _decorations;
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new IceProjectile(controller, owner, position, modifiers);
    }
}
