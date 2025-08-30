using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public class ZapCollision : Collision
    {
        private static SpriteWrapper _sprite = new SpriteWrapper(TextureService.Get("Sprites/projectiles/zap.png"), 4, .04f);

        protected override float Duration => 1f;
        public override bool AnimateManually => true;

        private readonly Vector2 _originalPosition;
        private readonly Vector2 _originalPlayerCenter;
        private Vector2 _lastMovement = new Vector2(0f,0f);

        public ZapCollision(Player player, Projectile projectile) 
            : base(player.Id, _sprite, projectile.Rect.Position, projectile.Rect.Size, new Vector2(0f,0f))
        {
            MoveTo(projectile.Rect.X, projectile.Rect.Y);
            ScaleTo(projectile.Rect.Width, projectile.Rect.Height);
            HurryTo(projectile.Speed.X, projectile.Speed.Y);
            _originalPosition = projectile.Rect.Position;
            _originalPlayerCenter = player.Center;
            SetRotationAndOffset();
        }

        public override void Update(Game game)
        {
            Move(Speed.X * Raylib.GetFrameTime(), Speed.Y * Raylib.GetFrameTime());
            SetRotationAndOffset();
            base.Update(game);
        }

        private void SetRotationAndOffset()
        {
            Rotation = DetermineZapRotation(_originalPlayerCenter);
            var offset = CalculateZapOffset(Rotation);
            Move(x: -_lastMovement.X, -_lastMovement.Y);
            Move(x: offset.X, y: offset.Y);
            _lastMovement = offset;
        }

        private float DetermineZapRotation(Vector2 playerCenter)
        {
            var direction = playerCenter - Center;
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
    }
}
