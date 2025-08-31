using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Particles
{
    public static class ParticleEffectFactory
    {
        /// <summary>
        /// Creates an explosion particle effect
        /// </summary>
        public static ParticleEmitter CreateExplosion(IObjectService objectService, Vector2 position, 
                                                     Guid owner, Color color = default, float intensity = 1f)
        {
            if (color.Equals(default(Color)))
                color = Color.Orange;

            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 200f * intensity,
                MaxParticles = (int)(50 * intensity),
                EmissionRadius = 5f,
                ParticleLifetime = 1.5f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(100f * intensity, 100f * intensity),
                ParticleSpeedVariation = new Vector2(50f * intensity, 50f * intensity),
                ParticleStartSize = 8f * intensity,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = color,
                ParticleEndColor = new Color((int)color.R, (int)color.G, (int)color.B, 0),
                Duration = 0.3f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                ParticleDrag = 2f
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates a projectile trail effect
        /// </summary>
        public static ParticleEmitter CreateProjectileTrail(IObjectService objectService, Vector2 position, 
                                                           Guid owner, Color color = default)
        {
            if (color.Equals(default(Color)))
                color = Color.Yellow;

            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Point,
                EmissionRate = 30f,
                MaxParticles = 20,
                ParticleLifetime = 0.8f,
                ParticleLifetimeVariation = 0.2f,
                ParticleSpeed = new Vector2(0f, 0f),
                ParticleSpeedVariation = new Vector2(10f, 10f),
                ParticleStartSize = 3f,
                ParticleEndSize = 0.5f,
                ParticleSizeVariation = 1f,
                ParticleStartColor = color,
                ParticleEndColor = new Color((int)color.R, (int)color.G, (int)color.B, 0),
                Duration = -1f, // Infinite
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates engine exhaust particles
        /// </summary>
        public static ParticleEmitter CreateEngineExhaust(IObjectService objectService, Vector2 position, 
                                                         Guid owner, bool isPlayer1)
        {
            Color exhaustColor = isPlayer1 ? Color.Blue : Color.Red;

            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 50f,
                MaxParticles = 30,
                EmissionRadius = 3f,
                ParticleLifetime = 0.6f,
                ParticleLifetimeVariation = 0.2f,
                ParticleSpeed = new Vector2(0f, 50f), // Exhaust goes down
                ParticleSpeedVariation = new Vector2(20f, 20f),
                ParticleStartSize = 4f,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 1f,
                ParticleStartColor = exhaustColor,
                ParticleEndColor = new Color((int)exhaustColor.R, (int)exhaustColor.G, (int)exhaustColor.B, 0),
                Duration = -1f, // Infinite
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates impact particles when projectiles hit something
        /// </summary>
        public static ParticleEmitter CreateImpact(IObjectService objectService, Vector2 position, 
                                                  Vector2 impactDirection, Guid owner, Color color = default)
        {
            if (color.Equals(default(Color)))
                color = Color.White;

            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Cone,
                EmissionRate = 150f,
                MaxParticles = 15,
                EmissionDirection = impactDirection,
                ConeAngle = 90f,
                ParticleLifetime = 0.5f,
                ParticleLifetimeVariation = 0.2f,
                ParticleSpeed = new Vector2(80f, 80f),
                ParticleSpeedVariation = new Vector2(40f, 40f),
                ParticleStartSize = 3f,
                ParticleEndSize = 0.5f,
                ParticleSizeVariation = 1f,
                ParticleStartColor = color,
                ParticleEndColor = new Color((int)color.R, (int)color.G, (int)color.B, 0),
                Duration = 0.2f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                UseGravity = true,
                GravityStrength = 200f,
                ParticleDrag = 1f
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates sparks effect
        /// </summary>
        public static ParticleEmitter CreateSparks(IObjectService objectService, Vector2 position, 
                                                  Guid owner, int sparkCount = 10)
        {
            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 200f,
                MaxParticles = sparkCount,
                EmissionRadius = 2f,
                ParticleLifetime = 1f,
                ParticleLifetimeVariation = 0.3f,
                ParticleSpeed = new Vector2(120f, 120f),
                ParticleSpeedVariation = new Vector2(60f, 60f),
                ParticleStartSize = 2f,
                ParticleEndSize = 0.2f,
                ParticleSizeVariation = 0.5f,
                ParticleStartColor = Color.Yellow,
                ParticleEndColor = Color.Orange,
                Duration = 0.1f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                UseGravity = true,
                GravityStrength = 150f,
                ParticleDrag = 0.5f
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates smoke particles
        /// /// </summary>
        public static ParticleEmitter CreateSmoke(IObjectService objectService, Vector2 position, 
                                                 Guid owner, float duration = 2f)
        {
            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 20f,
                MaxParticles = 25,
                EmissionRadius = 8f,
                ParticleLifetime = 3f,
                ParticleLifetimeVariation = 1f,
                ParticleSpeed = new Vector2(0f, -30f), // Smoke rises
                ParticleSpeedVariation = new Vector2(15f, 10f),
                ParticleStartSize = 5f,
                ParticleEndSize = 15f, // Smoke expands
                ParticleSizeVariation = 3f,
                ParticleStartColor = new Color(100, 100, 100, 150),
                ParticleEndColor = new Color(150, 150, 150, 0),
                Duration = duration,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                ParticleDrag = 0.8f
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates magical/energy particles for special effects
        /// </summary>
        public static ParticleEmitter CreateMagicalEffect(IObjectService objectService, Vector2 position, 
                                                         Guid owner, Color color = default)
        {
            if (color.Equals(default(Color)))
                color = Color.Magenta;

            var config = new ParticleEmitterConfig
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
                ParticleStartColor = color,
                ParticleEndColor = new Color((int)color.R, (int)color.G, (int)color.B, 0),
                Duration = -1f, // Infinite
                Loop = true,
                EmitOnStart = true,
                AutoDestroy = false,
                ParticleDrag = 3f // Floaty movement
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates ice crystal particles for ice effects
        /// </summary>
        public static ParticleEmitter CreateIceCrystals(IObjectService objectService, Vector2 position, 
                                                       Guid owner)
        {
            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 25f,
                MaxParticles = 20,
                EmissionRadius = 10f,
                ParticleLifetime = 1.5f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(40f, 40f),
                ParticleSpeedVariation = new Vector2(30f, 30f),
                ParticleStartSize = 3f,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 1f,
                ParticleStartColor = Color.SkyBlue,
                ParticleEndColor = new Color(135, 206, 235, 0),
                Duration = 1f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                UseGravity = true,
                GravityStrength = 50f
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates fire trail particles for ship movement
        /// </summary>
        public static ParticleEmitter CreateFireTrail(IObjectService objectService, Vector2 position, 
                                                     Guid owner, bool isPlayer1)
        {
            // Fire should always be fire colors - red/orange/yellow
            Color fireColor = Color.Orange;
            Color fireEndColor = new Color(255, 50, 0, 0); // Orange to red fade

            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 60f,
                MaxParticles = 25,
                EmissionRadius = 4f,
                ParticleLifetime = 0.8f,
                ParticleLifetimeVariation = 0.3f,
                ParticleSpeed = new Vector2(0f, -40f), // Fire trails behind ship
                ParticleSpeedVariation = new Vector2(25f, 15f),
                ParticleStartSize = 6f,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 2f,
                ParticleStartColor = fireColor,
                ParticleEndColor = fireEndColor,
                Duration = -1f, // Infinite
                Loop = true,
                EmitOnStart = false, // Start stopped, enable when moving
                AutoDestroy = false,
                ParticleDrag = 1.2f,
                UseGravity = false
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }

        /// <summary>
        /// Creates intense fire trail particles for fast movement
        /// </summary>
        public static ParticleEmitter CreateIntenseFireTrail(IObjectService objectService, Vector2 position, 
                                                            Guid owner, bool isPlayer1)
        {
            // Intense fire uses brighter, hotter colors
            Color primaryColor = Color.Yellow; // Hot fire starts yellow/white
            Color secondaryColor = Color.Red; // Fades to red
            
            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 80f,
                MaxParticles = 35,
                EmissionRadius = 6f,
                ParticleLifetime = 1.2f,
                ParticleLifetimeVariation = 0.4f,
                ParticleSpeed = new Vector2(0f, -60f), // Faster trail for intense movement
                ParticleSpeedVariation = new Vector2(30f, 20f),
                ParticleStartSize = 8f,
                ParticleEndSize = 0.5f,
                ParticleSizeVariation = 3f,
                ParticleStartColor = primaryColor,
                ParticleEndColor = new Color((int)secondaryColor.R, (int)secondaryColor.G, (int)secondaryColor.B, 0),
                Duration = -1f, // Infinite
                Loop = true,
                EmitOnStart = false,
                AutoDestroy = false,
                ParticleDrag = 0.8f,
                UseGravity = false
            };

            return new ParticleEmitter(owner, objectService, position, config);
        }
    }
}