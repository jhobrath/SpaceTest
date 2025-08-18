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
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;
        public PlayerStats Stats { get; set; } = new PlayerStats();
        public PlayerDisplay Display { get; set; }

        public bool IsPlayer1 { get; private set; }
        private List<PlayerEffect> _effects = new List<PlayerEffect>();
        private PlayerEffect _selectedProjectile;
        private PlayerEffect _defaultProjectile;

        public Player(Guid owner, PlayerDisplay display, bool isPlayer1)
            : base(owner, display.Sprite, display.Rect.Position, display.Rect.Size, new Vector2(0,0))
        {
            Display = display;
            IsPlayer1 = isPlayer1;
        }

        public void Collide(Projectile projectile)
        {
            IPlayerCollisionBehavior? collisionBehavior = null;
            foreach (var effect in _effects)
            {
                if (effect.IsProjectile && effect != _selectedProjectile)
                    continue;

                collisionBehavior = effect.CollisionBehavior ?? collisionBehavior;
            }

            collisionBehavior = collisionBehavior ?? _defaultProjectile.CollisionBehavior;
            collisionBehavior?.Apply(this, projectile);
        }

        public override void Update(Game game)
        {
            var stats = new PlayerStats();
            var display = new PlayerDisplay(Display.Sprite, Display.Rect, Display.Rotation);

            IPlayerInputBehavior? inputBehavior = null;
            IPlayerMovementBehavior? movementBehavior = null;
            IPlayerShootingBehavior? shootingBehavior = null;

            foreach(var effect in _effects)
            {
                if (effect.IsProjectile && effect != _selectedProjectile)
                    continue;

                effect.Apply(stats);
                effect.Apply(display);

                inputBehavior = effect.InputBehavior ?? inputBehavior;
                shootingBehavior = effect.ShootingBehavior ?? shootingBehavior;
                movementBehavior = effect.MovementBehavior ?? movementBehavior;
            }

            inputBehavior ??= _defaultProjectile.InputBehavior;
            shootingBehavior ??= _defaultProjectile.ShootingBehavior;
            movementBehavior ??= _defaultProjectile.MovementBehavior;

            Stats = stats;

            var input = inputBehavior?.Apply(this);
            var movement = movementBehavior?.Apply(this, input);
            shootingBehavior?.Apply(this, input, movement);

            if(input?.Switch?.IsPressed ?? false)
                SwitchProjectile();

            Display = display;
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }

        private void SwitchProjectile()
        {
            var selected = _effects.IndexOf(_selectedProjectile);

            for(var i = selected+1;i < _effects.Count;i++)
            {
                if (_effects[i].IsProjectile)
                { 
                    _selectedProjectile = _effects[i];
                    return;
                }
            }

            for(var i = 0;i < selected;i++)
            {
                if (_effects[i].IsProjectile)
                {
                    _selectedProjectile = _effects[i];
                    return;
                }
            }
        }

        public void AddEffect(PlayerEffect effect) 
        {
            if (_effects.All(x => !x.IsProjectile))
            {
                _defaultProjectile = effect;
                _selectedProjectile = effect;
            }

            if(effect.IsProjectile)
            {
                var duplicates = _effects.Where(x => x.GetType() == effect.GetType()).ToList();
                if(duplicates.Contains(_selectedProjectile))
                    _selectedProjectile = effect;

                foreach(var duplicate in duplicates)
                    _effects.Remove(duplicate);
            }

            _effects.Add(effect); 
        }
    }
}
