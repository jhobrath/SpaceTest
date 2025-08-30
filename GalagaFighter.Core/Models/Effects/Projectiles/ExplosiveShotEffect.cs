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
            modifiers.Projectile.OnShoot = HandleShotFired;
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Projectile.DamageMultiplier *= Raylib.GetFrameTime();
            modifiers.Projectile.DeactivateOnCollision = false;
            modifiers.Projectile.RotationOffsetIncrement = 360f;
            modifiers.Projectile.RotationOffsetMultiplier = 1/2f;

            modifiers.Projectile.Phases.Add(this, new List<float> { 1.6f, 1.65f, 2.0f, 2.05f, 2.15f, 2.25f });
            modifiers.Projectile.OnPhaseChange.Add(this, HandlePhaseChange);
            modifiers.Decorations = _decorations;
        }

        private void HandlePhaseChange(Projectile projectile, int phase)
        {
            if(phase == 1)
            { 
                AudioService.PlayExplosionConversionSound();
                projectile.Modifiers.Sprite = new SpriteWrapper(TextureService.Get("Sprites/Collisions/default.png"), 38, .02f, repeat: false);
                projectile.Modifiers.SizeMultiplier = new Vector2(5.6f,5.6f);
                projectile.Modifiers.DamageMultiplier = 1;
                projectile.Modifiers.SpeedMultiplier = .95f;
                projectile.IsMagnetic = false;
                projectile.Modifiers.OnCollide = (a, b) =>
                {
                    projectile.Modifiers.DamageMultiplier = 0f;
                    return [];
                };
            }
            else if(phase == 2)
            {
                projectile.Modifiers.DamageMultiplier = Raylib.GetFrameTime();
                projectile.Modifiers.OnCollide = null;
            }
            else if (phase == 2)
            {
                projectile.Modifiers.DamageMultiplier = Raylib.GetFrameTime();
                projectile.Modifiers.Opacity = .7f;
            }
            else if (phase == 3)
            {
                projectile.Modifiers.Opacity = .3f;
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
