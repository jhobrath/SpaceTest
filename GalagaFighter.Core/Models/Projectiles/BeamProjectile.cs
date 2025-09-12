using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class BeamProjectile : Projectile
    {
        public static Vector2 _baseSize => new(10f, 60f);
        public static Vector2 _baseSpeed => new(0f, 0f);
        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed;
        public override int BaseDamage => _baseDamage;
        public override Vector2 SpawnOffset => new(0, 0);

        private int _baseDamage = 0;
        private float _damageTimer = 0.1f;
        private const float _damageInterval = 0.1f;
        private Player _owner;
        private float _beamWidth = 10f;
        private const float _beamGrowthSpeed = 8000f;
        private float _originalX;

        public BeamProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers, Color? color)
            : base(controller, owner, GetSprite(), new Vector2(initialPosition.X, initialPosition.Y - _baseSize.Y/2), _baseSize, _baseSpeed, modifiers)
        {
            _owner = owner;
            _originalX = Position.X;
            AudioService.PlayShootSound();
            SetDrawPriority(-2);
            Color = Color.Red;
        }

        private static SpriteWrapper GetSprite()
        {
            return new SpriteWrapper("MissingSprites");
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            AudioService.PlayHitSound();
            return [new DefaultCollision(player.Id, initialPosition, initialSize, initialSpeed)];
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();

            _beamWidth += _beamGrowthSpeed * frameTime;
            ScaleTo(x: _beamWidth, y: Rect.Height);
            
            if (_owner.IsPlayer1)
            {
                MoveTo(x: _originalX);
            }
            else
            {
                MoveTo(x: _originalX - Rect.Width);
            }

            _damageTimer -= frameTime;
            if (_damageTimer <= 0f)
            {
                _damageTimer = _damageInterval;
                _baseDamage = 5;
            }
            else
            {
                _baseDamage = 0;
            }

            base.Update(game);
        }

        public override void Draw()
        {
            var rect = new Rectangle(Position.X, Position.Y, Rect.Width, Rect.Height);
            Raylib.DrawRectangleRec(rect, Color.Red);
            Raylib.DrawRectangleLinesEx(rect, 2f, Color.Yellow);
        }
    }
}
