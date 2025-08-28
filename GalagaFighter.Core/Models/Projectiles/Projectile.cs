using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
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

        public virtual bool IsMagnetic { get; set; } = true;

        private readonly IProjectileController _projectileController;
        public PlayerProjectile Modifiers { get; private set; }
        public Rectangle CurrentFrameRect { get; set; }
        //public Vector2 CurrentFrameSpeed { get; set; }

        public float Lifetime { get; set; } = 0f;

        protected Projectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed, PlayerProjectile modifiers) 
            : base(owner.Id, sprite, initialPosition, initialSize, new Vector2(initialSpeed.X * (owner.IsPlayer1 ? 1: -1), 0f))
        {
            if (!modifiers.IgnoreShipMovement)
                HurryTo(y: owner.Speed.Y / 6);

            _projectileController = controller;
            Modifiers = modifiers;
        }

        public override void Update(Game game)
        {
            _projectileController.Update(game, this);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, Rotation, CurrentFrameRect.Width, CurrentFrameRect.Height, Color);
        }

        public virtual List<PlayerEffect> CreateEffects() => [];
        public virtual List<Collision> CreateCollisions(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) => [];

    }
}
