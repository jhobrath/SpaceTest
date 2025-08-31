using GalagaFighter.Core.Controllers;
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
    public class ExplosiveProjectile : Projectile
    {
        private static Vector2 _baseSize => new(50f, 50f);
        private static Vector2 _baseSpeed => new(1020f, 0f);
        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => _baseDamage;
        public override Vector2 SpawnOffset => new(-80, 34);

        private float _explosionTime = 0f;
        private bool _isExploded = false;
        private float _slowTime = 0f;
        public override float? OnNearEdgeDistance => 110f;
        public override Action? OnNearEdge => HandleNearEdge;
        public override Action<Player>? OnCollide => HandleCollide;


        private int _baseDamage = 0;
        private float _damageTimer = 0f;
        private bool _fullHitApplied = false;
        private bool _fullHitLastFrame = false;

        public ExplosiveProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/explosion.png");
            return new SpriteWrapper(texture, 2, 1000f);
        }

        private void HandleNearEdge()
        {
            if (_isExploded) 
                return;

            _baseDamage = 10;

            _isExploded = true;
            _explosionTime = 3.0f;
            AudioService.PlayExplosionConversionSound();
            var explosionSprite = new SpriteWrapper(TextureService.Get("Sprites/Collisions/default.png"), 38, .02f, repeat: false);
            Sprite = explosionSprite;
            Hurry(.25f);
            Scale(5.6f, 5.6f);
            Move(-Rect.Width / 2, -Rect.Height / 2);
            CurrentFrameRect = Rect;
            IsMagnetic = false;
        }

        private void HandleCollide(Player player)
        {
            if (_baseDamage == 10)
                _fullHitApplied = true;

            _baseDamage = 0;
        }

        public override void Update(Game game)
        {
            base.Update(game);
            if (!_isExploded)
                return;

            if (_explosionTime <= 0f)
            {
                Modifiers.Opacity = 0f;
                IsActive = false;
                return;
            }

            if(_damageTimer > (_fullHitApplied ? .15f : .06f))
            {
                _baseDamage = 1;
                _damageTimer = 0f;
            }
            else
                _baseDamage = 0;

            _damageTimer += Raylib.GetFrameTime();
            _explosionTime -= Raylib.GetFrameTime();
            Modifiers.Opacity = Math.Clamp(_explosionTime / 3.0f, 0f, 1f);
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [new BurningEffect()];
        }
    }
}
