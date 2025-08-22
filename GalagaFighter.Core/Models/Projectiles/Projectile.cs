using GalagaFighter.Core.Controllers;
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
        public abstract Vector2 BaseSpeed { get; }
        public abstract Vector2 BaseSize { get; }
        public abstract int BaseDamage { get; }

        public abstract Vector2 SpawnOffset { get; }

        public PlayerProjectile Modifiers { get; private set; }
        private readonly IProjectileController _projectileController;

        public virtual SpriteWrapper? CollisionSprite => null;

        protected Projectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed, PlayerProjectile modifiers) 
            : base(owner.Id, sprite, initialPosition, initialSize, new Vector2(initialSpeed.X * (owner.IsPlayer1 ? 1: -1), owner.CurrentFrameSpeed.Y/3))
        {
            _projectileController = controller;
            Modifiers = modifiers;
        }

        public override void Update(Game game)
        {
            _projectileController.Update(game, this);
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
