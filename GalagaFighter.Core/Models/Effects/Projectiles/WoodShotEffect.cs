using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class WoodShotEffect : PlayerEffect
    {
        private readonly SpriteWrapper _sprite;
        private readonly SpriteDecorations _decorations;

        public override bool IsProjectile => true;
        public override string IconPath => "Sprites/Effects/woodshot.png";
        protected override int TotalBullets => 3;

        public WoodShotEffect(Color? color)
        {
            _sprite = new SpriteWrapper("Sprites/Ships/MainShip.png", color ?? Color.White);

            _decorations = new SpriteDecorations()
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWoodGuns.png"))),
                //ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_ShootBoth.png"))),
                //ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_ShootLeft.png"))),
                //ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_ShootRight.png"))),
                //WindUpLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_WindUpLeft.png"), 3, .125f)),
                //WindUpRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_WindUpRight.png"), 3, .125f)),
                //WindUpBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWood_WindUpBoth.png"), 3, .125f)),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
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
            modifiers.Projectile.IgnoreShipMovement = true;
            modifiers.Decorations = _decorations;
        }

        private Projectile CreateProjectile(IProjectileController projectileController, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new WoodProjectile(projectileController, owner, position, modifiers);
    }
}
