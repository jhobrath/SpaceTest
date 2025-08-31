using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Models
{
    /// <summary>
    /// Complete particle effect definition with full configuration control
    /// </summary>
    public class ParticleEffect : IEquatable<ParticleEffect>
    {
        // Position and Behavior
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public bool FollowRotation { get; set; } = false;

        // Emission Configuration
        public EmissionShape Shape { get; set; } = EmissionShape.Point;
        public float EmissionRate { get; set; } = 30f;
        public int MaxParticles { get; set; } = 20;
        public float EmissionRadius { get; set; } = 0f;
        public Vector2 EmissionDirection { get; set; } = Vector2.Zero;
        public float ConeAngle { get; set; } = 45f;

        // Particle Lifecycle
        public float ParticleLifetime { get; set; } = 1f;
        public float ParticleLifetimeVariation { get; set; } = 0.2f;
        public float Duration { get; set; } = -1f; // -1 = infinite
        public bool Loop { get; set; } = true;
        public bool EmitOnStart { get; set; } = true;
        public bool AutoDestroy { get; set; } = false;

        // Particle Movement
        public Vector2 ParticleSpeed { get; set; } = new Vector2(0f, 0f);
        public Vector2 ParticleSpeedVariation { get; set; } = new Vector2(10f, 10f);
        public bool UseGravity { get; set; } = false;
        public float GravityStrength { get; set; } = 100f;
        public float ParticleDrag { get; set; } = 0f;

        // Particle Appearance
        public float ParticleStartSize { get; set; } = 4f;
        public float ParticleEndSize { get; set; } = 1f;
        public float ParticleSizeVariation { get; set; } = 1f;
        public Color ParticleStartColor { get; set; } = Color.White;
        public Color ParticleEndColor { get; set; } = new Color(255, 255, 255, 0);

        // Sprite Configuration - Full control!
        public List<string> Sprites { get; set; } = new List<string>();
        public SpriteSelectionMode SpriteSelection { get; set; } = SpriteSelectionMode.Random;

        // Optional name for debugging/library
        public string Name { get; set; } = "";

        public ParticleEffect()
        {
        }

        public ParticleEffect(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Copy constructor for easy cloning and modification
        /// </summary>
        public ParticleEffect(ParticleEffect source)
        {
            CopyFrom(source);
        }

        public void CopyFrom(ParticleEffect source)
        {
            Offset = source.Offset;
            FollowRotation = source.FollowRotation;
            Shape = source.Shape;
            EmissionRate = source.EmissionRate;
            MaxParticles = source.MaxParticles;
            EmissionRadius = source.EmissionRadius;
            EmissionDirection = source.EmissionDirection;
            ConeAngle = source.ConeAngle;
            ParticleLifetime = source.ParticleLifetime;
            ParticleLifetimeVariation = source.ParticleLifetimeVariation;
            Duration = source.Duration;
            Loop = source.Loop;
            EmitOnStart = source.EmitOnStart;
            AutoDestroy = source.AutoDestroy;
            ParticleSpeed = source.ParticleSpeed;
            ParticleSpeedVariation = source.ParticleSpeedVariation;
            UseGravity = source.UseGravity;
            GravityStrength = source.GravityStrength;
            ParticleDrag = source.ParticleDrag;
            ParticleStartSize = source.ParticleStartSize;
            ParticleEndSize = source.ParticleEndSize;
            ParticleSizeVariation = source.ParticleSizeVariation;
            ParticleStartColor = source.ParticleStartColor;
            ParticleEndColor = source.ParticleEndColor;
            Sprites = new List<string>(source.Sprites);
            SpriteSelection = source.SpriteSelection;
            Name = source.Name;
        }

        /// <summary>
        /// Calculate hash code for caching - includes all properties that affect rendering
        /// </summary>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            // Removed orientation-dependent properties from hash for stable caching
            hash.Add(Shape);
            hash.Add(EmissionRate);
            hash.Add(MaxParticles);
            hash.Add(EmissionRadius);
            hash.Add(EmissionDirection);
            hash.Add(ConeAngle);
            hash.Add(ParticleLifetime);
            hash.Add(ParticleLifetimeVariation);
            hash.Add(Duration);
            hash.Add(Loop);
            hash.Add(EmitOnStart);
            hash.Add(AutoDestroy);
            hash.Add(ParticleSpeed);
            hash.Add(ParticleSpeedVariation);
            hash.Add(UseGravity);
            hash.Add(GravityStrength);
            hash.Add(ParticleDrag);
            hash.Add(ParticleStartSize);
            hash.Add(ParticleEndSize);
            hash.Add(ParticleSizeVariation);
            hash.Add(ParticleStartColor.R);
            hash.Add(ParticleStartColor.G);
            hash.Add(ParticleStartColor.B);
            hash.Add(ParticleStartColor.A);
            hash.Add(ParticleEndColor.R);
            hash.Add(ParticleEndColor.G);
            hash.Add(ParticleEndColor.B);
            hash.Add(ParticleEndColor.A);
            hash.Add(SpriteSelection);
            // Include sprites in hash
            foreach (var sprite in Sprites.OrderBy(s => s))
            {
                hash.Add(sprite);
            }
            return hash.ToHashCode();
        }

        public bool Equals(ParticleEffect other)
        {
            if (other == null) return false;
            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ParticleEffect);
        }

        /// <summary>
        /// Get cache key for this effect combined with object ID
        /// </summary>
        public string GetCacheKey(Guid objectId, bool fromModifiers = false)
        {
            return $"{(fromModifiers ? "modifiers_" : "")}{objectId}_{GetHashCode():X8}";
        }
    }

    /// <summary>
    /// How to select sprites when multiple are provided
    /// </summary>
    public enum SpriteSelectionMode
    {
        Random,      // Pick random sprite for each particle
        Sequential,  // Cycle through sprites in order
        First        // Always use first sprite
    }

    public enum EmissionShape
    {
        Point,      // Emit from a single point
        Circle,     // Emit from around a circle
        Rectangle,  // Emit from within a rectangle
        Line,       // Emit along a line
        Cone        // Emit in a cone direction
    }
}