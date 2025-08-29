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
        private readonly SpriteWrapper _sprite;
        protected override float Duration => 5f;

        private SpriteDecorations _decorations;

        public MudShotEffect(Color? color)
        {
            _sprite = new SpriteWrapper("Sprites/Ships/MainShip.png", color ?? Color.White);
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
            modifiers.Sprite = _sprite;
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Decorations = _decorations;
            
            // ✅ Phase transformation (like explosive)
            modifiers.Projectile.Phases.Add(this, new List<float> { 2f, 4f, 4.25f, 4.5f, 4.75f, 5f });  // Transform at 95% completion
            modifiers.Projectile.OnPhaseChange = HandlePhaseChange;
            modifiers.Projectile.DeactivateOnCollision = false;      // Stay active for continuous collision
        }

        private void HandlePhaseChange(Projectile projectile, PlayerEffect playerEffect, int phase)
        {
            if (playerEffect != this)
                return;

            if(phase == 1)
            { 
                AudioService.PlayMudSplat();
                // Transform into immobile mud splat
                int frameIndex = Game.Random.Next(0, 3); // Randomly pick frame 0, 1, or 2
                var baseTexture = TextureService.Get("Sprites/Projectiles/mud_splat.png");
                var mudSplatTexture = TextureService.GetFrame(baseTexture, 3, frameIndex); // Extract the frame as Texture2D
                var mudSplatSprite = new SpriteWrapper(mudSplatTexture);
                mudSplatSprite.Color = Raylib_cs.Color.White.ApplyAlpha(.8f);
                
                projectile.Modifiers.Sprite = mudSplatSprite;
                projectile.Modifiers.SizeMultiplier = new Vector2(3f,6f);   // Make it huge
                projectile.Modifiers.SpeedMultiplier = 0f;   // Make it immobile
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