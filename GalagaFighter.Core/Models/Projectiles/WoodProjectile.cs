using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
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

        public WoodProjectile(IProjectileUpdater updater, Player owner, Vector2 initialPosition)
            : base(updater, owner, GetSprite(BaseSize), initialPosition, BaseSize, BaseSpeed)
        {
        }

        private static SpriteWrapper GetSprite(Vector2 size)
        {
            return new SpriteWrapper(SpriteGenerationService.CreateProjectileSprite(ProjectileType.Wall, (int)size.X, (int)size.Y));
        }
    }
}