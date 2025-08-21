using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class DefaultProjectile : Projectile
    {
        public static Vector2 BaseSize => new(30f, 15f);
        public static Vector2 BaseSpeed => new(1020f, 0f);
        public override int BaseDamage => 5;
        public override Vector2 SpawnOffset => new Vector2(-10, 30);

        public DefaultProjectile(IProjectileUpdater updater, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(updater, owner, GetSprite(BaseSize), initialPosition, BaseSize, BaseSpeed, modifiers)
        {
        }

        private static SpriteWrapper GetSprite(Vector2 initialSize)
        {
            return new SpriteWrapper(SpriteGenerationService.CreateProjectileSprite(ProjectileType.Normal, (int)initialSize.X, (int)initialSize.Y));
        }
    }
}