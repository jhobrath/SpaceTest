using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class IceProjectile : Projectile
    {
        public static Vector2 BaseSize => new(95f, 42f);
        public static Vector2 BaseSpeed => new(1020f, 0f);

        public override int Damage => 0;

        public IceProjectile(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(owner, GetSprite(), initialPosition, initialSize, initialSpeed)
        {
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/ice.png");
            return new SpriteWrapper(texture, 6, .33f);
        }

        public override List<PlayerEffect> CreateEffects(IObjectService objectService)
        {
            return new List<PlayerEffect> { new FrozenEffect() };
        }
    }
}