using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class ExplosiveProjectile : Projectile
    {
        private static Vector2 _baseSize => new(50f, 50f);
        private static Vector2 _baseSpeed => new(1020f, 0f);
        
        public override Vector2 BaseSpeed => _baseSize;
        public override Vector2 BaseSize => _baseSpeed;
        public override int BaseDamage => 50;

        public override Vector2 SpawnOffset => new Vector2(-50, 15);

        private readonly SpriteWrapper _explodeSprite = new SpriteWrapper(TextureService.Get("Sprites/Collisions/default.png"), 38, .04f, repeat: false);
        private float _explodeTimer = (float)Game.Random.NextDouble() * .3f + 1.1f;

        public ExplosiveProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/explosion.png"); //Never use frame 2
            return new SpriteWrapper(texture, 2, 1000f);
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [new BurningEffect()];
        }
    }
}
