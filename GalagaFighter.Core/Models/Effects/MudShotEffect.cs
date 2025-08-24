using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class MudShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/mudshot.png";
        public override bool IsProjectile => true;
        private readonly SpriteWrapper _sprite;
        protected override float Duration => 5f;

        private SpriteDecorations _decorations;

        private float _mudSplatOpacity = 0.5f;

        public MudShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud.png"));
            _decorations = new SpriteDecorations
            {
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_ShootRight.png"), 3, .6f)),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_ShootLeft.png"), 3, .6f)),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_ShootBoth.png"), 3, .6f)),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.Projectiles.Add(CreateProjectile);
            modifiers.Decorations = _decorations;
            
            // ✅ Phase transformation (like explosive)
            modifiers.Projectile.Phases = new List<float> { 2f, 4f, 4.25f, 4.5f, 4.75f, 5f };  // Transform at 95% completion
            modifiers.Projectile.OnPhaseChange = HandlePhaseChange;
            modifiers.Projectile.DeactivateOnCollision = false;      // Stay active for continuous collision
        }

        private void HandlePhaseChange(Projectile projectile, int phase)
        {
            if(phase == 1)
            { 
                AudioService.PlayMudSplat();
                // Transform into immobile mud splat
                var mudSplatSprite = new SpriteWrapper(TextureService.Get("Sprites/Projectiles/mud_splat.png"), 3, 10f); // 10 second frame duration
                mudSplatSprite.CurrentFrame = Game.Random.Next(0, 3); // Randomly pick frame 0, 1, or 2
                mudSplatSprite.Color = Raylib_cs.Color.White.ApplyAlpha(.8f);
            
                projectile.Modifiers.Sprite = mudSplatSprite;
                projectile.Modifiers.SizeMultiplier = new Vector2(3f,6f);   // Make it huge
                projectile.Modifiers.SpeedMultiplier = 0f;   // Make it immobile
                projectile.Modifiers.Opacity = .5f;
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