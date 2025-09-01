using GalagaFighter.Core.Models;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Static
{
    /// <summary>
    /// Library of pre-configured particle effects that can be used as templates
    /// </summary>
    public static class ParticleEffectsLibrary
    {
        private static readonly Dictionary<string, ParticleEffect> _effects = new();

        static ParticleEffectsLibrary()
        {
            InitializeEffects();
        }

        /// <summary>
        /// Get a copy of a library effect that can be modified
        /// </summary>
        public static ParticleEffect Get(string name)
        {
            if (_effects.TryGetValue(name, out var effect))
            {
                return new ParticleEffect(effect); // Return a copy
            }
            throw new ArgumentException($"Particle effect '{name}' not found in library");
        }

        /// <summary>
        /// Check if an effect exists in the library
        /// </summary>
        public static bool Contains(string name)
        {
            return _effects.ContainsKey(name);
        }

        /// <summary>
        /// Get all available effect names
        /// </summary>
        public static IEnumerable<string> GetAvailableEffects()
        {
            return _effects.Keys;
        }

        private static void InitializeEffects()
        {
            // Ice Trail Effect
            _effects["IceTrail"] = new ParticleEffect("IceTrail")
            {
                EmissionRate = 25f,
                MaxParticles = 15,
                ParticleLifetime = 1.2f,
                ParticleLifetimeVariation = 0.3f,
                ParticleSpeed = Vector2.Zero,
                ParticleSpeedVariation = new Vector2(10f, 10f),
                ParticleStartSize = 8f,
                ParticleEndSize = 2f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = Color.SkyBlue,
                ParticleEndColor = new Color(135, 206, 235, 0),
                Duration = -1f,
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                Sprites = { "star_5", "star_4" }, // Sharp ice crystals
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = new Vector2(-20f, 0f),
                FollowRotation = true
            };

            // Ice Aura Effect
            _effects["IceAura"] = new ParticleEffect("IceAura")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 15f,
                MaxParticles = 12,
                EmissionRadius = 20f,
                ParticleLifetime = 2.5f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(15f, 15f),
                ParticleSpeedVariation = new Vector2(20f, 20f),
                ParticleStartSize = 6f,
                ParticleEndSize = 10f,
                ParticleSizeVariation = 4f,
                ParticleStartColor = new Color(200, 230, 255, 180),
                ParticleEndColor = new Color(200, 230, 255, 0),
                Duration = -1f,
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                ParticleDrag = 2f,
                Sprites = { "star_4", "star_3" }, // Medium sharp crystals
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = Vector2.Zero,
                FollowRotation = false
            };

            // Ice Impact Effect
            _effects["IceImpact"] = new ParticleEffect("IceImpact")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 100f,
                MaxParticles = 30,
                EmissionRadius = 12f,
                ParticleLifetime = 2f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(60f, 60f),
                ParticleSpeedVariation = new Vector2(40f, 40f),
                ParticleStartSize = 10f,
                ParticleEndSize = 3f,
                ParticleSizeVariation = 4f,
                ParticleStartColor = Color.White,
                ParticleEndColor = new Color(135, 206, 235, 0),
                Duration = 1.2f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                UseGravity = true,
                GravityStrength = 80f,
                Sprites = { "star_5" }, // Maximum sharpness for impact
                SpriteSelection = SpriteSelectionMode.First,
                Offset = Vector2.Zero,
                FollowRotation = false
            };

            // Fire Trail Effect
            _effects["FireTrail"] = new ParticleEffect("FireTrail")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 60f,
                MaxParticles = 25,
                EmissionRadius = 4f,
                ParticleLifetime = 0.8f,
                ParticleLifetimeVariation = 0.3f,
                ParticleSpeed = new Vector2(0f, -40f),
                ParticleSpeedVariation = new Vector2(25f, 15f),
                ParticleStartSize = 6f,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 2f,
                ParticleStartColor = Color.Orange,
                ParticleEndColor = new Color(255, 50, 0, 0),
                Duration = -1f,
                Loop = true,
                EmitOnStart = true, // FIXED: Changed to true so particles start automatically
                AutoDestroy = false,
                ParticleDrag = 1.2f,
                Sprites = { "dot_4", "dot_5", "dot_3" }, // Round dots for fire
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = new Vector2(-10f, 0f),
                FollowRotation = true
            };

            // Intense Fire Trail Effect  
            _effects["IntenseFireTrail"] = new ParticleEffect("IntenseFireTrail")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 80f,
                MaxParticles = 35,
                EmissionRadius = 6f,
                ParticleLifetime = 1.2f,
                ParticleLifetimeVariation = 0.4f,
                ParticleSpeed = new Vector2(0f, -60f),
                ParticleSpeedVariation = new Vector2(30f, 20f),
                ParticleStartSize = 8f,
                ParticleEndSize = 0.5f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = Color.Yellow, // Hot fire starts yellow
                ParticleEndColor = new Color(255, 0, 0, 0), // Fades to red
                Duration = -1f,
                Loop = true,
                EmitOnStart = true, // FIXED: Changed to true so particles start automatically
                AutoDestroy = false,
                ParticleDrag = 0.8f,
                Sprites = { "dot_4", "dot_5", "dot_3" }, // Round dots for intense fire
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = new Vector2(-12f, 0f),
                FollowRotation = true
            };

            // Explosion Effect
            _effects["Explosion"] = new ParticleEffect("Explosion")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 200f,
                MaxParticles = 50,
                EmissionRadius = 5f,
                ParticleLifetime = 1.5f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(100f, 100f),
                ParticleSpeedVariation = new Vector2(50f, 50f),
                ParticleStartSize = 8f,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = Color.Orange,
                ParticleEndColor = new Color(255, 165, 0, 0),
                Duration = 0.3f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                ParticleDrag = 2f,
                Sprites = { "dot_4", "dot_5" }, // Hard dots for explosion
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = Vector2.Zero,
                FollowRotation = false
            };

            // Magical Sparkles Effect
            _effects["MagicalSparkles"] = new ParticleEffect("MagicalSparkles")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 40f,
                MaxParticles = 30,
                EmissionRadius = 15f,
                ParticleLifetime = 2f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(30f, 30f),
                ParticleSpeedVariation = new Vector2(20f, 20f),
                ParticleStartSize = 4f,
                ParticleEndSize = 8f,
                ParticleSizeVariation = 2f,
                ParticleStartColor = Color.Magenta,
                ParticleEndColor = new Color(255, 0, 255, 0),
                Duration = -1f,
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                ParticleDrag = 3f,
                Sprites = { "star_3", "star_4", "dot_3" }, // Mix of stars and dots
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = Vector2.Zero,
                FollowRotation = false
            };

            // Electric Trail Effect
            _effects["ElectricTrail"] = new ParticleEffect("ElectricTrail")
            {
                Shape = EmissionShape.Point,
                EmissionRate = 40f,
                MaxParticles = 20,
                ParticleLifetime = 0.6f,
                ParticleLifetimeVariation = 0.2f,
                ParticleSpeed = new Vector2(0f, -30f),
                ParticleSpeedVariation = new Vector2(20f, 10f),
                ParticleStartSize = 10f,
                ParticleEndSize = 3f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = Color.SkyBlue,
                ParticleEndColor = Color.White, // Keep bright white instead of transparent
                Duration = -1f,
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                ParticleDrag = 0.5f,
                Sprites = { "lightning_1", "lightning_2", "lightning_3", "lightning_4", "lightning_5" }, // Simple to medium lightning + sparks
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = new Vector2(-15f, 0f),
                FollowRotation = true
            };

            // Electric Zap Effect (for when electric projectile gets near player)
            _effects["ElectricZap"] = new ParticleEffect("ElectricZap")
            {
                Shape = EmissionShape.Line,
                EmissionRate = 80f,
                MaxParticles = 40,
                EmissionRadius = 30f, // Length of the line
                ParticleLifetime = 0.3f,
                ParticleLifetimeVariation = 0.1f,
                ParticleSpeed = Vector2.Zero, // Static sparks
                ParticleSpeedVariation = new Vector2(5f, 5f),
                ParticleStartSize = 12f,
                ParticleEndSize = 6f,
                ParticleSizeVariation = 4f,
                ParticleStartColor = Color.White,
                ParticleEndColor = new Color(0, 255, 255, 0),// Keep visible instead of transparent
                Duration = 0.5f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                Sprites = { "lightning_4", "lightning_5", "spark_5" }, // Complex lightning + intense sparks
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = Vector2.Zero,
                FollowRotation = false
            };

            // Electric Impact Effect
            _effects["ElectricImpact"] = new ParticleEffect("ElectricImpact")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 150f,
                MaxParticles = 50,
                EmissionRadius = 8f,
                ParticleLifetime = 1f,
                ParticleLifetimeVariation = 0.3f,
                ParticleSpeed = new Vector2(80f, 80f),
                ParticleSpeedVariation = new Vector2(40f, 40f),
                ParticleStartSize = 8f,
                ParticleEndSize = 2f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = Color.Yellow,
                ParticleEndColor = Color.White, // Keep bright instead of transparent
                Duration = 0.8f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                Sprites = { "spark_4", "spark_5", "lightning_3" }, // Intense sparks + medium lightning
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = Vector2.Zero,
                FollowRotation = false
            };

            // Connected Lightning Chain Effect  
            _effects["LightningChain"] = new ParticleEffect("LightningChain")
            {
                Shape = EmissionShape.Point,
                EmissionRate = 8f, // Fewer chains but more dramatic
                MaxParticles = 3, // Max 3 chains at once
                ParticleLifetime = 0.4f, // Chain duration
                ParticleLifetimeVariation = 0.1f,
                ParticleSpeed = Vector2.Zero, // Chains don't move
                ParticleSpeedVariation = Vector2.Zero,
                ParticleStartSize = 32f, // Segment size
                ParticleEndSize = 32f,
                ParticleSizeVariation = 0f,
                ParticleStartColor = Color.White,
                ParticleEndColor = new Color(0, 255, 255, 0),
                Duration = -1f,
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                Sprites = { "lightning_1", "lightning_2" }, // Only need these two!
                SpriteSelection = SpriteSelectionMode.Random,
                Offset = new Vector2(-16f, 0f),
                FollowRotation = true
            };

            // Flamethrower Beam Effect
            _effects["FlamethrowerBeam"] = new ParticleEffect("FlamethrowerBeam")
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 120f, // High emission for thick beam
                MaxParticles = 100, // Increased for longer beam coverage
                EmissionRadius = 8f, // Spread for beam thickness
                ParticleLifetime = 3.5f, // EXTENDED: Much longer to reach screen edge
                ParticleLifetimeVariation = 0.2f, // Reduced variation for consistency
                ParticleSpeed = new Vector2(200f, 0f), // Forward motion for beam
                ParticleSpeedVariation = new Vector2(40f, 60f), // Random spread outward
                ParticleStartSize = 12f, // Large particles for visibility
                ParticleEndSize = 3f, // Shrink as they burn out
                ParticleSizeVariation = 4f,
                ParticleStartColor = Color.Yellow, // Hot fire starts yellow
                ParticleEndColor = new Color(255, 80, 0, 0), // Fades to orange then transparent
                Duration = -1f,
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                UseGravity = false, // No gravity as requested
                ParticleDrag = 0.3f, // Minimal drag to maintain beam shape
                Sprites = { "fire_1", "fire_2", "fire_3", "fire_4", "fire_5" }, // Your fire textures
                SpriteSelection = SpriteSelectionMode.Random, // CHANGED: Use Random to avoid cycling gaps
                Offset = new Vector2(20f, 0f), // Offset forward from source
                FollowRotation = true
            };
        }
    }
}