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
        private readonly SpriteDecorations _decorations;

        public override bool IsProjectile => true;
        public override string IconPath => "Sprites/Effects/woodshot.png";
        protected override int TotalBullets => 3;

        public WoodShotEffect()
        {
            _decorations = new SpriteDecorations()
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipWoodGuns.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
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
