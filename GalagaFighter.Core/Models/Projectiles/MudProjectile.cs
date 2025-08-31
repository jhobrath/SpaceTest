using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class MudProjectile : Projectile
    {
        private static readonly Vector2 _baseSpeed = new(800f, 0f);
        private static readonly Vector2 _baseSize = new(150f, 64f);

        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 0; // Mud doesn't do damage
        public override Vector2 SpawnOffset => new(-75, 20);

        private float _splatTime = 0f;
        private bool _isSplatted = false;

        public override float? OnNearEdgeDistance => 60f;
        public override Action? OnNearEdge => HandleNearEdge;


        public MudProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/mud.png");
            return new SpriteWrapper(texture, 6, .12f);
        }

        private void HandleNearEdge()
        {
            if (_isSplatted) 
                return;

            Hurry(0f, 0f);
            _isSplatted = true;
            _splatTime = 3.0f;
            AudioService.PlayMudSplat();
            int frameIndex = Game.Random.Next(0, 3);
            var baseTexture = TextureService.Get("Sprites/Projectiles/mud_splat.png");
            var mudSplatTexture = TextureService.GetFrame(baseTexture, 3, frameIndex);
            var mudSplatSprite = new SpriteWrapper(mudSplatTexture);
            mudSplatSprite.Color = Raylib_cs.Color.White.ApplyAlpha(.8f);
            Modifiers.Sprite = mudSplatSprite;
            Modifiers.SizeMultiplier = new Vector2(3f,6f);
            Modifiers.SpeedMultiplier = 0f;
            Modifiers.Opacity = 1f;
            IsMagnetic = false;
        }

        public override void Update(Game game)
        {
            base.Update(game);
            if (_isSplatted)
            {
                if (_splatTime > 0f)
                {
                    _splatTime -= Raylib.GetFrameTime();
                    Modifiers.Opacity = Math.Clamp(_splatTime / 3.0f, 0f, 1f);
                }
                else
                {
                    Modifiers.Opacity = 0f;
                    IsActive = false;
                }
            }
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            // Mud projectiles don't create traditional collisions
            return [];
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [new MudSlowEffect()];
        }
    }
}