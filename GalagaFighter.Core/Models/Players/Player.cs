using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;
        public PlayerStats Stats { get; set; } = new PlayerStats();
        public PlayerDisplay Display { get; set; }
        public List<PlayerEffect> StatusEffects = new List<PlayerEffect>();
        public List<PlayerEffect> ProjectileEffects = new List<PlayerEffect>();
        public PlayerEffect? SelectedProjectileEffect = null;

        public bool IsPlayer1 { get; private set; }

        public Player(Guid owner, PlayerDisplay display, bool isPlayer1)
            : base(owner, display.Sprite, display.Rect.Position, display.Rect.Size, new Vector2(0,0))
        {
            Display = display;
            IsPlayer1 = isPlayer1;
        }

        public void Collide(Projectile projectile)
        {
            IPlayerCollisionBehavior? collisionBehavior = ProjectileEffects[0].CollisionBehavior;
            foreach (var effect in StatusEffects)
                collisionBehavior = effect.CollisionBehavior ?? collisionBehavior;

            collisionBehavior = SelectedProjectileEffect?.CollisionBehavior ?? collisionBehavior;
            collisionBehavior?.Apply(this, projectile);
        }

        public override void Update(Game game)
        {
            var stats = new PlayerStats();
            var display = new PlayerDisplay(Display.Sprite, Display.Rect, Display.Rotation);

            IPlayerInputBehavior? inputBehavior = ProjectileEffects[0].InputBehavior;
            IPlayerMovementBehavior? movementBehavior = ProjectileEffects[0].MovementBehavior;
            IPlayerShootingBehavior? shootingBehavior = ProjectileEffects[0].ShootingBehavior;

            void apply(PlayerEffect effect)
            {
                effect.Apply(stats);
                effect.Apply(display);
                if (effect.InputBehavior != null) inputBehavior = effect.InputBehavior;
                if (effect.MovementBehavior != null) movementBehavior = effect.MovementBehavior;
                if (effect.ShootingBehavior != null) shootingBehavior = effect.ShootingBehavior;
            }

            foreach (var effect in StatusEffects)
                apply(effect);

            apply(SelectedProjectileEffect ?? ProjectileEffects[0]);

            Stats = stats;
            Display = display;

            var input = inputBehavior?.Apply(this);
            var movement = movementBehavior?.Apply(this, input);
            shootingBehavior?.Apply(this, input, movement);

            if (input?.Switch?.IsPressed == true)
                SwitchProjectile();
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }

        private void SwitchProjectile()
        {
            var selected = ProjectileEffects.Contains(SelectedProjectileEffect)
                ? ProjectileEffects.IndexOf(SelectedProjectileEffect)
                : 0;

            if (ProjectileEffects.Count > selected + 1)
            {
                SelectedProjectileEffect = ProjectileEffects[selected + 1];
                return;
            }

            SelectedProjectileEffect = ProjectileEffects[0];
        }
    }
}
