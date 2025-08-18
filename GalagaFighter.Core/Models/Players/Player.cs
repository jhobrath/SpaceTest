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
using System.Drawing;
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

            IPlayerInputBehavior? inputBehavior = InputBehavior;
            IPlayerMovementBehavior? movementBehavior = MovementBehavior;
            IPlayerShootingBehavior? shootingBehavior = ShootingBehavior;

            foreach(var effect in Effects)
            {
                effect.Apply(stats);
                effect.Apply(display);

                inputBehavior = effect.InputBehavior ?? inputBehavior;
                shootingBehavior = effect.ShootingBehavior ?? shootingBehavior;
                movementBehavior = effect.MovementBehavior ?? movementBehavior;
            }

            Stats = stats;

            var input = inputBehavior?.Apply(this);
            var movement = movementBehavior?.Apply(this, input);
            shootingBehavior?.Apply(this, input, movement);

            Display = display;
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }

        public void SetMovementBehavior(IPlayerMovementBehavior movementBehavior) => MovementBehavior = movementBehavior;
        public void SetCollisionBehavior(IPlayerCollisionBehavior collisionBehavior) => CollisionBehavior = collisionBehavior;
        public void SetShootingBehavior(IPlayerShootingBehavior shootingBehavior) => ShootingBehavior = shootingBehavior;
        public void SetInputBehavior(IPlayerInputBehavior inputBehavior) => InputBehavior = inputBehavior;
        public void AddEffect(PlayerEffect effect) { Effects.Add(effect); }
    }
}
