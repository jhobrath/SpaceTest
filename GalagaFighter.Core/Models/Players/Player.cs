using GalagaFighter.Core.Behaviors;
using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;
        public PlayerStats Stats { get; set; } = new PlayerStats();
        public PlayerDisplay Display { get; set; }

        public IPlayerShootingBehavior? ShootingBehavior { get; set; } 
        public IPlayerInputBehavior? InputBehavior { get; set; }
        public IPlayerMovementBehavior? MovementBehavior { get; set; }
        public IPlayerCollisionBehavior? CollisionBehavior { get; set; }

        public bool IsPlayer1 { get; private set; }
        private List<Projectile> _collisions = [];

        private PlayerStats _baseStats;
        private PlayerDisplay _baseDisplay;


        public Player(Guid owner, PlayerDisplay display, bool isPlayer1)
            : base(owner, display.Sprite, display.Rect.Position, display.Rect.Size, new Vector2(0,0))
        {
            Display = display;
            IsPlayer1 = isPlayer1;

            _baseDisplay = new PlayerDisplay(display.Sprite, display.Rect, display.Rotation);
            _baseStats = new PlayerStats();
        }

        public void SetCollisions(List<Projectile> projectiles)
        {
            _collisions = projectiles;
        }

        public void Collide(Projectile projectile)
        {
            CollisionBehavior?.Apply(this, projectile);
        }

        public override void Update(Game game)
        {
            //Always start with base stats/rendering
            Stats = new PlayerStats();
            Display = new PlayerDisplay(_baseDisplay.Sprite, _baseDisplay.Rect, _baseDisplay.Rotation);

            var input = InputBehavior?.Apply(new PlayerInputUpdate());
            var movement = MovementBehavior?.Apply(this, input);
            ShootingBehavior?.Apply(this, input, movement);
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }

        public void SetMovementBehavior(PlayerMovementBehavior movementBehavior) => MovementBehavior = movementBehavior;
        public void SetCollisionBehavior(PlayerCollisionBehavior collisionBehavior) => CollisionBehavior = collisionBehavior;
        public void SetShootingBehavior(PlayerShootingBehavior shootingBehavior) => ShootingBehavior = shootingBehavior;
        public void SetInputBehavior(PlayerInputBehavior inputBehavior) => InputBehavior = inputBehavior;
    }
}
