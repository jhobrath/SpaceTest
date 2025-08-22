using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class WoodProjectile : Projectile
    {
        public static Vector2 BaseSize => new(150f, 15f);
        public static Vector2 BaseSpeed => new(-250f, 0f);
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => new Vector2(-40, 25);

        public bool Released { get; set; } = false;
        public bool Planked { get; set; } = false;

        public WoodProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(BaseSize), initialPosition, BaseSize, BaseSpeed, modifiers)
        {
        }

        private static SpriteWrapper GetSprite(Vector2 size)
        {
            return new SpriteWrapper(SpriteGenerationService.CreateProjectileSprite(ProjectileType.Wall, (int)size.X, (int)size.Y));
        }

        public override List<Collision> CreateCollisions(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            return new List<Collision>
            {
                new DefaultCollision(owner, initialPosition, initialSize, initialSpeed)
            };
        }
    }
}