using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class WoodShotEffect : PlayerEffect
    {
        private readonly SpriteWrapper _sprite;
        private readonly SpriteDecorations _decorations;

        public override bool IsProjectile => true;
        public override string IconPath => "Sprites/Effects/woodshot.png";
        protected override int TotalBullets => 3;

        public WoodShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood.png"));

            _decorations = new SpriteDecorations()
            {
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_ShootRight.png"))),
                WindUpLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_WindUpLeft.png"), 3, .125f)),
                WindUpRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_WindUpRight.png"), 3, .125f)),
                WindUpBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_WindUpBoth.png"), 3, .125f)),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.OnWindUpReleased = HandleShotFired;
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Projectile.WindUpDuration = 1.0f;
            modifiers.Projectile.WindUpSpeed = 250f;
            modifiers.Projectile.PlankDuration = 7f;
            modifiers.Projectile.PlankStopsMovement = true;

            modifiers.Decorations = _decorations;
        }

        private Projectile CreateProjectile(IProjectileController projectileController, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new WoodProjectile(projectileController, owner, position, modifiers);
    }
}
