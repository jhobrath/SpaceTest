using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
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
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => new Vector2(-10, 30);

        public IceProjectile(IProjectileUpdater updater, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(updater, owner, GetSprite(), initialPosition, BaseSize, BaseSpeed, modifiers)
        {
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/ice.png");
            return new SpriteWrapper(texture, 6, .33f);
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return new List<PlayerEffect> { new FrozenEffect() };
        }
    }
}