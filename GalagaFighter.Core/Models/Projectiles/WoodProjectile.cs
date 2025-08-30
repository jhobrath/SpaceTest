using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class WoodProjectile : Projectile
    {
        private static Vector2 _baseSize = new(150f, 15f);
        private static Vector2 _baseSpeed= new(7000f, 0f);

        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed;
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => new(-40, 45);

        public bool Released { get; set; } = false;
        public bool Planked { get; set; } = false;

        public WoodProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
        }

        private static SpriteWrapper GetSprite()
        {
            return new SpriteWrapper(SpriteGenerationService.CreateProjectileSprite(ProjectileType.Wall, (int)_baseSize.X, (int)_baseSize.Y));
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            return
            [
                new DefaultCollision(player.Id, initialPosition, initialSize, initialSpeed)
            ];
        }
    }
}