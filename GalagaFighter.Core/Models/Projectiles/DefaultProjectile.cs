using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class DefaultProjectile : Projectile
    {
        private static Vector2 _baseSize => new(30f, 15f);
        private static Vector2 _baseSpeed => new(2000f, 0f);
        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed;
        public override int BaseDamage => 5;
        public override Vector2 SpawnOffset => new(-60, 42);

        public DefaultProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers, Color? color)
            : base(controller, owner, GetSprite(_baseSize, color), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
        }

        private static SpriteWrapper GetSprite(Vector2 initialSize, Color? color)
        {
            return new SpriteWrapper(SpriteGenerationService.CreateProjectileSprite(ProjectileType.Normal, (int)initialSize.X, (int)initialSize.Y, color));
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            AudioService.PlayHitSound();
            return
            [
                new DefaultCollision(player.Id, initialPosition, initialSize, initialSpeed)
            ];
        }
    }
}