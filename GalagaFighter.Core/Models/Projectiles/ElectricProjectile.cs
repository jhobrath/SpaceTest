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
    public class ElectricProjectile : Projectile
    {
        private static Vector2 _baseSize => new(60f, 60f);
        private static Vector2 _baseSpeed => new(250f, 0f);

        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed; 
        public override int BaseDamage => 8;
        public override Vector2 SpawnOffset => new(-70, 42);

        public ElectricProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/electric.png");
            return new SpriteWrapper(texture, 5, .125f);
        }

        public override void Update(Game game)
        {
            base.Update(game);
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return  [new ElectricEffect()];
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            return
            [
                new ZapCollision(player, this)
            ];
        }
    }
}