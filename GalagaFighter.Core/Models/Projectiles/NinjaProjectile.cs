using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class NinjaProjectile : Projectile
    {
        private static readonly Vector2 _baseSpeed = new Vector2(1080f, 40f);
        private static readonly Vector2 _baseSize = new Vector2(38.8f, 27.77f);
        
        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 7;
        public override Vector2 SpawnOffset => new Vector2(-50, 15);


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

        public override List<Collision> CreateCollisions(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            var collision = new BurstCollision(owner, initialPosition, initialSize, initialSpeed);
            collision.Color = collision.Color.ShiftHueForTexture(40f);
            collision.Color = collision.Color.Darken(.4f);
            return new List<Collision>
            {
                collision
            };
        }

    }
}
