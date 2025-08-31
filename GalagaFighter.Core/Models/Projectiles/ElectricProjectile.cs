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
    public class ElectricProjectile : Projectile
    {
        private static Vector2 _baseSize => new(60f, 60f);
        private static Vector2 _baseSpeed => new(950f, 0f);

        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed; 
        public override int BaseDamage => _baseDamage;
        public override Vector2 SpawnOffset => new(-70, 42);

        private int _baseDamage = 8;

        public override float? OnNearPlayerDistance => 105f;
        public override Action<Player> OnNearPlayer => StartZap;

        private bool _isZapping = false;
        private Vector2 _zapOriginalPosition;
        private Vector2 _zapOriginalPlayerCenter;
        private Vector2 _zapLastOffset;

        public ElectricProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/electric.png");
            return new SpriteWrapper(texture, 5, .125f);
        }

        private void StartZap(Player player)
        {
            if (_isZapping)
                return;

            _isZapping = true;
            Sprite = new SpriteWrapper(TextureService.Get("Sprites/projectiles/zap.png"), 4, .04f);
            _zapOriginalPosition = Rect.Position;
            _zapOriginalPlayerCenter = player.Center;
            _zapLastOffset = Vector2.Zero;

            UpdateZap();
        }

        private void UpdateZap()
        {
            Rotation = DetermineZapRotation(_zapOriginalPlayerCenter, Center);
            var offset = CalculateZapOffset(Rotation);
            Move(x: -_zapLastOffset.X, -_zapLastOffset.Y);
            Move(x: offset.X, y: offset.Y);
            _zapLastOffset = offset;
        }

        private float DetermineZapRotation(Vector2 playerCenter, Vector2 projectileCenter)
        {
            var direction = playerCenter - projectileCenter;
            float angleRadians = MathF.Atan2(direction.Y, direction.X);
            float angleDegrees = angleRadians * (180f / MathF.PI);
            return angleDegrees;
        }

        private Vector2 CalculateZapOffset(float angleDegrees)
        {
            // Base offset for 45 degree contact
            Vector2 baseOffset = new Vector2(24f, -24f);
            float angleRadians = angleDegrees * (MathF.PI / 180f);
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);
            // Rotate the base offset by the given angle
            float x = baseOffset.X * cos - baseOffset.Y * sin;
            float y = baseOffset.X * sin + baseOffset.Y * cos;
            return new Vector2(x, y);
        }

        public override void Update(Game game)
        {
            base.Update(game);
            if (_isZapping)
            {
                UpdateZap();
                _baseDamage = 0;
            }
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return  [new ElectricEffect()];
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            return [];
        }
    }
}