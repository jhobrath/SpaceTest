using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class NinjaProjectile : Projectile
    {
        private static readonly Vector2 _baseSpeed = new(2080f, 40f);
        private static readonly Vector2 _baseSize = new(38.8f, 27.77f);
        
        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 7;
        public override Vector2 SpawnOffset => new(-50, 42);


        public NinjaProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayNinjaShot();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/ninja.png");
            return new SpriteWrapper(texture, 3, .05f);
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            var collision = new BurstCollision(player.Id, initialPosition, initialSize, initialSpeed);
            collision.Color = collision.Color.ShiftHueForTexture(40f);
            collision.Color = collision.Color.Darken(.4f);
            return
            [
                collision
            ];
        }

    }
}
