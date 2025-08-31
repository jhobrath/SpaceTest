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
    public class ExplosiveShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/explosiveshot.png";
        public override bool IsProjectile => true;
        protected override int TotalBullets => 5;
        private SpriteDecorations _decorations;

        public ExplosiveShotEffect()
        {
            _decorations = new SpriteDecorations()
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipExplosiveGuns.png"))),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns_ShootRight.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Decorations = _decorations;
            modifiers.Projectile.DeactivateOnCollision = false;
            modifiers.Projectile.OnShoot = HandleShotFired;
        }

        private Projectile CreateProjectile(IProjectileController projectileController, Player owner, Vector2 position, PlayerProjectile modifiers)
        {
            return new ExplosiveProjectile(projectileController, owner, position, modifiers);
        }
    }
}
