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
            
            modifiers.Projectile.Phases.Add(this, new List<float> { 2f, 4f, 4.25f, 4.5f, 4.75f, 5f });
            modifiers.Projectile.OnPhaseChange.Add(this, HandlePhaseChange);
            modifiers.Projectile.DeactivateOnCollision = false;
        }

        private void HandlePhaseChange(Projectile projectile, int phase)
        {
            if(phase == 1)
            { 
                AudioService.PlayMudSplat();
                int frameIndex = Game.Random.Next(0, 3);
                var baseTexture = TextureService.Get("Sprites/Projectiles/mud_splat.png");
                var mudSplatTexture = TextureService.GetFrame(baseTexture, 3, frameIndex);
                var mudSplatSprite = new SpriteWrapper(mudSplatTexture);
                mudSplatSprite.Color = Raylib_cs.Color.White.ApplyAlpha(.8f);
                projectile.Modifiers.Sprite = mudSplatSprite;
                projectile.Modifiers.SizeMultiplier = new Vector2(3f,6f);
                projectile.Modifiers.SpeedMultiplier = 0f;
                projectile.Modifiers.Opacity = .5f;
                projectile.IsMagnetic = false;
            }
            else if(phase == 2)
                projectile.Modifiers.Opacity = .4f;
            else if(phase == 3)
                projectile.Modifiers.Opacity = .3f;
            else if(phase == 4)
                projectile.Modifiers.Opacity = .2f;
            else if (phase == 5)
                projectile.Modifiers.Opacity = .1f;
            else if (phase == 6)
            { 
                projectile.Modifiers.Opacity = 0f;
                projectile.IsActive = false;
            }
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new MudProjectile(controller, owner, position, modifiers);
    }
}