using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class MudShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/mudshot.png";
        public override bool IsProjectile => true;
        protected override float Duration => 5f;

        private SpriteDecorations _decorations;

        public MudShotEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMudGuns.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMudGuns_ShootRight.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMudGuns_ShootLeft.png"))),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMudGuns_ShootBoth.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Decorations = _decorations;
            modifiers.Projectile.DeactivateOnCollision = false;
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new MudProjectile(controller, owner, position, modifiers);
    }
}