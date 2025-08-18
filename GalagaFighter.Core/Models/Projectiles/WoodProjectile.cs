using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class WoodProjectile : Projectile
    {
        public static Vector2 BaseSize => new(150f, 15f);
        public static Vector2 BaseSpeed => new(-250f, 0f);

        public override int Damage => 0;
        public bool Released { get; set; } = false;
        public bool Planked { get; set; } = false;

        public WoodProjectile(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(owner, GetSprite(initialSize), initialPosition, initialSize, initialSpeed)
        {
        }

        private static SpriteWrapper GetSprite(Vector2 size)
        {
            return new SpriteWrapper(SpriteGenerationService.CreateProjectileSprite(ProjectileType.Wall, (int)size.X, (int)size.Y));
        }

        //public override List<PlayerEffect> CreateEffects(IObjectService objectService)
        //{
        //  return new List<PlayerEffect> { new FrozenEffect() };
        //}
    }
}