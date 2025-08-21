using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Behaviors.Projectiles.Interfaces;
using GalagaFighter.Core.Behaviors.Projectiles.Updates;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public abstract int BaseDamage { get; }

        protected virtual IProjectileMovementBehavior? MovementBehavior { get; set; }
        protected virtual IProjectileDestroyBehavior? DestroyBehavior { get; set; }
        protected virtual IProjectileCollisionBehavior? CollisionBehavior { get; set; }
        public abstract Vector2 SpawnOffset { get; }

        public PlayerProjectile Modifiers { get; private set; }
        private readonly IProjectileUpdater _projectileUpdater;

        public virtual SpriteWrapper? CollisionSprite => null;

        protected Projectile(IProjectileUpdater projectileUpdater, Player owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed, PlayerProjectile modifiers) 
            : base(owner.Id, sprite, initialPosition, initialSize, new Vector2(initialSpeed.X * (owner.IsPlayer1 ? 1: -1), owner.CurrentFrameSpeed.Y/3))
        {
            _projectileUpdater = projectileUpdater;
            Modifiers = modifiers;
        }

        public override void Update(Game game)
        {
            _projectileUpdater.Update(game, this);
            Sprite.Update(Raylib.GetFrameTime());
        }

        public override void Draw()
        {
            Sprite.Draw(Center, Rotation, Rect.Width, Rect.Height, Color);
        }

        public virtual List<PlayerEffect> CreateEffects() => [];
        public virtual List<Collision> CreateCollisions(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) => [];

    }
}
