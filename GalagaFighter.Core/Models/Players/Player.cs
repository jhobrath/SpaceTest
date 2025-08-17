using GalagaFighter.Core.Behaviors;
using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections;
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
        public List<PlayerEffect> Effects { get; set; }


        public Player(Guid owner, PlayerDisplay display, bool isPlayer1)
            : base(owner, display.Sprite, display.Rect.Position, display.Rect.Size, new Vector2(0,0))
        {
            Display = display;
            IsPlayer1 = isPlayer1;
            Effects = new List<PlayerEffect>();
        }

        public void Collide(PowerUp powerUp)
        {
            Effects.AddRange(powerUp.Effects);
        }

        public void Collide(Projectile projectile)
        {
            IPlayerCollisionBehavior? _collisionBehavior = CollisionBehavior;
            foreach (var effect in Effects)
                _collisionBehavior = effect.CollisionBehavior ?? _collisionBehavior;

            _collisionBehavior?.Apply(this, projectile);
        }

        public override void Update(Game game)
        {
            var stats = new PlayerStats();
            var display = new PlayerDisplay(Display.Sprite, Display.Rect, Display.Rotation);

            IPlayerInputBehavior? _inputBehavior = InputBehavior;
            IPlayerMovementBehavior? _movementBehavior = MovementBehavior;
            IPlayerShootingBehavior? _shootingBehavior = ShootingBehavior;

            foreach(var effect in Effects)
            {
                effect.Apply(stats);
                effect.Apply(display);

                _inputBehavior = effect.InputBehavior ?? _inputBehavior;
                _shootingBehavior = effect.ShootingBehavior ?? _shootingBehavior;
                _movementBehavior = effect.MovementBehavior ?? _movementBehavior;
            }

            Stats = stats;

            var input = _inputBehavior?.Apply();
            var movement = MovementBehavior?.Apply(this, input);
            ShootingBehavior?.Apply(this, input, movement);

            Display = display;
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }

        public void SetMovementBehavior(PlayerMovementBehavior movementBehavior) => MovementBehavior = movementBehavior;
        public void SetCollisionBehavior(PlayerCollisionBehavior collisionBehavior) => CollisionBehavior = collisionBehavior;
        public void SetShootingBehavior(PlayerShootingBehavior shootingBehavior) => ShootingBehavior = shootingBehavior;
        public void SetInputBehavior(PlayerInputBehavior inputBehavior) => InputBehavior = inputBehavior;
        public void AddEffect(PlayerEffect effect) { Effects.Add(effect); }
    }
}
