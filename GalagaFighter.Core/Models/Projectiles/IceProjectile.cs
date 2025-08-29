using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class IceProjectile : Projectile
    {
        private static Vector2 _baseSize => new(95f, 42f);
        private static Vector2 _baseSpeed => new(1020f, 0f);

        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed; 
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => new(-50, 42);

        public IceProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
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

        public override List<Collision> CreateCollisions(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            var size = Math.Clamp(initialSize.Y, 50f, 200f);
            return
            [
                new IceShotCollision(owner, initialPosition, new Vector2(size,size), initialSpeed)
            ];
        }
    }
}