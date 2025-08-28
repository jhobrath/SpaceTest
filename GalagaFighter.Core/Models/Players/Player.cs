using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject, IDrawnPlayer
    {
        public float Health { get; set; } = 100f;

        public PlayerStats BaseStats { get; private set; } = new PlayerStats();
        public Color? PalleteSwap { get; set; }
        public bool IsPlayer1 { get; private set; }

        public Func<PlayerEffect>? OffensiveAugment { get; set; }
        public Func<PlayerEffect>? DefensiveAugment { get; set; }

        private readonly IPlayerController _playerController;

        public Player(IPlayerController playerUpdater, Rectangle initialPosition, bool isPlayer1)
            : base(Game.Id, 
                  new SpriteWrapper("Sprites/Ships/MainShip.png"), 
                  initialPosition.Position, 
                  initialPosition.Size, 
                  new Vector2(0,20f))
        {
            _playerController = playerUpdater;
            IsPlayer1 = isPlayer1;
            
            // Set the hitbox for this player to use triangle collision with percentage coordinates
            Hitbox = new HitboxTriangle([
                new Vector2(0.045f, 0.685f),  // Left wing: 4.5% in, 68.5% down
                new Vector2(0.955f, 0.685f),  // Right wing: 95.5% in, 68.5% down
                new Vector2(0.5f, 0.08f)      // Ship tip: 50% in, 8% down
            ]);
        }

        public void Initialize(float health, PlayerStats stats, Color palleteSwap)
        {
            Health = health;
            BaseStats = stats;
            PalleteSwap = palleteSwap;
        }

        public override void Update(Game game)
        {
            _playerController.Update(game, this);
        }

        public override void Draw()
        {
            _playerController.Draw(this);
        }
    }


    public interface IDrawnPlayer
    {
        public Vector2 Center { get; }
        public Vector2 Speed { get; }
        public float Rotation { get; }
    }

    public class Phantom : IDrawnPlayer
    {
        private Vector2 _center;
        private Vector2 _speed;
        private readonly Player _owner;

        public Vector2 Center => _center;
        public Vector2 Speed => _speed;
        public float Rotation => GetRotation();

        private float GetRotation()
        {
            if (_speed.Y < 0 && _owner.Speed.Y > 0)
                return _owner.Rotation - 90f * (_owner.IsPlayer1 ? 1 : -1);
            else if(_speed.Y > 0 && _owner.Speed.Y < 0)
                return _owner.Rotation + 90f * (_owner.IsPlayer1 ? 1 : -1);
            else
                return _owner.Rotation;
        }

        public Phantom(Player owner)
        {
            _owner = owner;
            _center = owner.Center;
            _speed = owner.Speed;
        }

        public void HurryTo(float? x = null, float? y = null)
        {
            _speed = new Vector2(x ?? _speed.X, y ?? _speed.Y);
        }

        public void Update()
        {
            var newY = _center.Y + _speed.Y * Raylib.GetFrameTime();
            if (newY - 160f < 0)
                _speed = new Vector2(_speed.X, -_speed.Y);
            else if (newY > Game.Height - 50f)
                _speed = new Vector2(_speed.X, -_speed.Y);

            _center = _center + _speed * Raylib.GetFrameTime();
        }
    }
}
