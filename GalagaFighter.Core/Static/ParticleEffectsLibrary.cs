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
        }
    }
}