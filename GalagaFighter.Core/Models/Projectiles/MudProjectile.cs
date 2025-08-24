using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class MudProjectile : Projectile
    {
        private static readonly Vector2 _baseSpeed = new Vector2(800f, 0f);
        private static readonly Vector2 _baseSize = new Vector2(150f, 64f);

        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 0; // Mud doesn't do damage
        public override Vector2 SpawnOffset => new Vector2(-75, 20);

        public MudProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayMudSplat();
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/mud.png");
            return new SpriteWrapper(texture, 6, .12f);
        }

        public override List<Collision> CreateCollisions(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            // Mud projectiles don't create traditional collisions
            return new List<Collision>();
        }

        public override List<PlayerEffect> CreateEffects()
        {
            return [];
        }
    }
}