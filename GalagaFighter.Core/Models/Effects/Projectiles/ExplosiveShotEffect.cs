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
        private SpriteWrapper _sprite;
        private SpriteDecorations _decorations;

        public ExplosiveShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipExplosive.png"));

            _decorations = new SpriteDecorations
            {
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipExplosive_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipExplosive_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipExplosive_ShootRight.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipExplosive_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.OnShoot = HandleShotFired;
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Projectile.DamageMultiplier *= Raylib.GetFrameTime();
            modifiers.Projectile.DeactivateOnCollision = false;
            modifiers.Projectile.RotationOffsetIncrement = 360f;
            modifiers.Projectile.RotationOffsetMultiplier = 1/2f;

            modifiers.Projectile.Phases.Add(this, new List<float> { 1.45f, 2.25f });
            modifiers.Projectile.OnPhaseChange = HandlePhaseChange;
            modifiers.Decorations = _decorations;
        }

        private void HandlePhaseChange(Projectile projectile, PlayerEffect playerEffect, int phase)
        {
            if (playerEffect.GetType() != GetType())
                return;

            if(phase == 1)
            { 
                AudioService.PlayExplosionConversionSound();
                projectile.Modifiers.Sprite = new SpriteWrapper(TextureService.Get("Sprites/Collisions/default.png"), 38, .02f, repeat: false);
                projectile.Modifiers.SizeMultiplier = new Vector2(5.6f,5.6f);
                projectile.Modifiers.SpeedMultiplier = .33f;
                projectile.IsMagnetic = false;
            }
            else
            {
                projectile.IsActive = false;
            }
        }

        private Projectile CreateProjectile(IProjectileController projectileController, Player owner, Vector2 position, PlayerProjectile modifiers)
        {
            return new ExplosiveProjectile(projectileController, owner, position, modifiers);
        }
    }
}
