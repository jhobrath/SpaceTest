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
        public IPlayerShootingBehavior ShootingBehavior { get; set; } = new DefaultShootingBehavior();
        public IPlayerInputBehavior InputBehavior { get; set; }
        public IPlayerMovementBehavior MovementBehavior { get; set; } = new DefaultMovementBehavior();
        public IPlayerCollisionBehavior CollisionBehavior { get; set; } = new PlayerCollisionBehavior();

        public bool IsPlayer1 { get; private set; }
        public List<Projectile> Projectiles { get; } = [];
        private List<Projectile> _collisions = [];

        private PlayerStats _baseStats;
        private PlayerDisplay _baseDisplay;


        public Player(IPlayerInputBehavior inputBehavior, PlayerDisplay display, bool isPlayer1)
            : base(display.Sprite, display.Rect.Position, display.Rect.Size, new Vector2(0,0))
        {
            InputBehavior = inputBehavior;
            Display = display;
            IsPlayer1 = isPlayer1;

            _baseDisplay = new PlayerDisplay(display.Sprite, display.Rect, display.Rotation);
            _baseStats = new PlayerStats();
        }

        public void SetCollisions(List<Projectile> projectiles)
        {
            _collisions = projectiles;
        }

        public override void Update(Game game)
        {
            //Always start with base stats/rendering
            Stats = new PlayerStats();
            Display = new PlayerDisplay(_baseDisplay.Sprite, _baseDisplay.Rect, _baseDisplay.Rotation);

            var input = InputBehavior.Apply(new PlayerInputUpdate());
            var movement = MovementBehavior.Apply(this, input, new PlayerMovementUpdate(this));
            var shooting = ShootingBehavior.Apply(this, input, movement, new PlayerShootingUpdate());
            var collisions = CollisionBehavior.Apply(this, new PlayerCollisionUpdate { Hits = _collisions });

            Projectiles.RemoveAll(p => !p.IsActive);

            foreach(var projectile in shooting.Projectiles)
            {
                Projectiles.Add(projectile);
                game.AddGameObject(projectile);
            }

            foreach (var projectile in collisions.Destroy)
                projectile.IsActive = false;

            foreach (var obj in collisions.Create)
                game.AddGameObject(obj);

            Health -= collisions.DamageDealt * Stats.Shield;

            MoveTo(movement.To.X, movement.To.Y);
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }
    }
}
