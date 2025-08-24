using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
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
            modifiers.Projectile.Projectiles.Add(CreateProjectile);
            modifiers.Projectile.DamageMultiplier *= Raylib.GetFrameTime();
            modifiers.Projectile.DeactivateOnCollision = false;
            modifiers.Projectile.RotationOffsetIncrement = 360f;
            modifiers.Projectile.RotationOffsetMultiplier = (1/2f);

            modifiers.Projectile.Phases = new List<float> { .95f };
            modifiers.Projectile.OnPhaseChange = HandlePhaseChange;
            modifiers.Decorations = _decorations;
        }

        private void HandlePhaseChange(Projectile projectile, int phase)
        {
            AudioService.PlayExplosionConversionSound();
            projectile.Modifiers.Sprite = new SpriteWrapper(TextureService.Get("Sprites/Collisions/default.png"), 38, .04f, repeat: false);
            projectile.Modifiers.SizeMultiplier = new Vector2(5.6f,5.6f);
            projectile.Modifiers.SpeedMultiplier = .5f;
        }

        private Projectile CreateProjectile(IProjectileController projectileController, Player owner, Vector2 position, PlayerProjectile modifiers)
        {
            return new ExplosiveProjectile(projectileController, owner, position, modifiers);
        }
    }
}
