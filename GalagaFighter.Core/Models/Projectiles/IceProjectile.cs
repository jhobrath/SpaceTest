using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class IceProjectile : Projectile
    {
        private static Vector2 _baseSize => new(95f, 42f);
        private static Vector2 _baseSpeed => new(2020f, 0f);

        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed; 
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => new(-50, 42);

        public IceProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();
            
            // Add particle effects using new full control system
            AddIceParticleEffects();
        }

        private void AddIceParticleEffects()
        {
            // Get base ice trail effect from library and customize
            var iceTrail = ParticleEffectsLibrary.Get("IceTrail");
            iceTrail.Offset = new Vector2(-20f, Speed.X > 0 ? -5f : 5);
            iceTrail.ParticleStartSize = 12f;
            iceTrail.ParticleEndSize = 2f;
            iceTrail.EmissionRate = 20;  // Increased emission rate
            iceTrail.MaxParticles = 20;    // INCREASED MAX PARTICLES - this was the bottleneck!
            iceTrail.ParticleLifetime = 1.5f; // Longer lifetime = longer trail
            iceTrail.Sprites = new List<string> { "dot_2", "dot_1" }; // Sharp stars
            ParticleEffects.Add(iceTrail);

            // Get base ice aura effect from library and customize
            var iceAura = ParticleEffectsLibrary.Get("IceAura");
            iceAura.ParticleStartColor = new Color(200, 230, 255, 180);
            iceAura.EmissionRate = 25f; // Increased aura particles
            iceAura.MaxParticles = 30;  // More aura particles
            iceAura.Sprites = new List<string> { "star_4", "star_3" }; // Medium sharp crystals
            ParticleEffects.Add(iceAura);
        }

        /// <summary>
        /// Handle impact - transition from trail effects to impact effect
        /// </summary>
        public void OnImpact()
        {
            // Remove trail and aura effects
            ClearParticleEffects();

            // Add impact effect with full control
            var impact = ParticleEffectsLibrary.Get("IceImpact");
            impact.Offset = Vector2.Zero; // At impact point
            impact.ParticleStartColor = Color.White; // Bright white crystals
            impact.ParticleStartSize = 12f; // Large impact crystals
            ParticleEffects.Add(impact);
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

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            OnImpact();
            var size = Math.Clamp(initialSize.Y, 50f, 200f);
            return
            [
                new IceShotCollision(player.Id, initialPosition, new Vector2(size,size), initialSpeed)
            ];
        }
    }
}