using GalagaFighter.Core.Models.Particles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Particles
{
    public class ParticleEmitter : GameObject
    {
        private readonly IObjectService _objectService;
        private readonly ParticleEffect _sourceEffect; // ? Keep reference to original effect
        private readonly List<Particle> _particles;
        private readonly List<string> _sprites;
        private readonly SpriteSelectionMode _spriteSelection;
        private float _emissionTimer;
        private float _durationTimer;
        private bool _isEmitting;
        private int _sequentialIndex;

        public ParticleEffect SourceEffect => _sourceEffect; // ? Expose the source effect
        public int ActiveParticleCount => _particles.Count;
        public bool IsEmitting => _isEmitting;

        public ParticleEmitter(Guid owner, IObjectService objectService, Vector2 position, 
                              ParticleEffect sourceEffect, SpriteWrapper particleSprite = null,
                              List<string> sprites = null, SpriteSelectionMode spriteSelection = SpriteSelectionMode.First) 
            : base(owner, particleSprite, position, Vector2.One, Vector2.Zero)
        {
            _objectService = objectService;
            _sourceEffect = sourceEffect ?? throw new ArgumentNullException(nameof(sourceEffect));
            _particles = new List<Particle>();
            _emissionTimer = 0f;
            _durationTimer = 0f;
            _isEmitting = _sourceEffect.EmitOnStart;
            _sprites = sprites ?? new List<string>();
            _spriteSelection = spriteSelection;
            _sequentialIndex = 0;
            SetDrawPriority(0.4); // Draw emitter before particles
        }

        public override void Update(Game game)
        {
            float frameTime = Raylib.GetFrameTime();

            // Update duration if not infinite - read live from source effect
            if (_sourceEffect.Duration > 0)
            {
                _durationTimer += frameTime;
                if (_durationTimer >= _sourceEffect.Duration)
                {
                    if (_sourceEffect.Loop)
                    {
                        _durationTimer = 0f;
                    }
                    else
                    {
                        _isEmitting = false;
                        if (_sourceEffect.AutoDestroy && _particles.Count == 0)
                        {
                            IsActive = false;
                            return;
                        }
                    }
                }
            }

            // Emit particles - read live emission rate and max particles
            if (_isEmitting)
            {
                _emissionTimer += frameTime;
                float emissionInterval = 1f / _sourceEffect.EmissionRate;
                
                while (_emissionTimer >= emissionInterval && _particles.Count < _sourceEffect.MaxParticles)
                {
                    EmitParticle();
                    _emissionTimer -= emissionInterval;
                }
            }

            // Update existing particles
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update(game);
                if (!_particles[i].IsActive)
                {
                    _objectService.RemoveGameObject(_particles[i]);
                    _particles.RemoveAt(i);
                }
            }
        }

        private void EmitParticle()
        {
            Vector2 emissionPosition = GetEmissionPosition();
            Vector2 particleSpeed = GetParticleSpeed();
            float lifetime = GetParticleLifetime();
            float startSize = GetParticleStartSize();
            float endSize = GetParticleEndSize();
            float rotation = GetParticleRotation();

            var particleSprite = GetParticleSprite();

            var particle = new Particle(
                Id, // Emitter is the owner
                particleSprite,
                emissionPosition,
                new Vector2(startSize, startSize),
                particleSpeed,
                lifetime,
                ApplyColorVariation(_sourceEffect.ParticleStartColor, _sourceEffect.ParticleColorVariation),
                ApplyColorVariation(_sourceEffect.ParticleEndColor, _sourceEffect.ParticleColorVariation),
                startSize,
                endSize
            )
            {
                Acceleration = _sourceEffect.ParticleAcceleration,
                UseGravity = _sourceEffect.UseGravity,
                GravityStrength = _sourceEffect.GravityStrength,
                Drag = _sourceEffect.ParticleDrag,
                Rotation = rotation
            };

            _particles.Add(particle);
            _objectService.AddGameObject(particle);
        }

        private float GetParticleRotation()
        {
            // Random rotation between 0 and 360 degrees
            return (float)(Game.Random.NextDouble() * 360.0);
        }

        protected virtual SpriteWrapper GetParticleSprite()
        {
            if (_sprites.Count > 0)
            {
                string selectedSprite = _spriteSelection switch
                {
                    SpriteSelectionMode.First => _sprites[0],
                    SpriteSelectionMode.Random => _sprites[Game.Random.Next(_sprites.Count)],
                    SpriteSelectionMode.Sequential => GetSequentialSprite(),
                    _ => _sprites[0]
                };
                return new SpriteWrapper($"Sprites/Particles/{selectedSprite}.png", _sourceEffect.ParticleStartColor);
            }
            return Sprite ?? CreateDefaultParticleSprite();
        }

        private string GetSequentialSprite()
        {
            string sprite = _sprites[_sequentialIndex];
            _sequentialIndex = (_sequentialIndex + 1) % _sprites.Count;
            return sprite;
        }

        private Vector2 GetEmissionPosition()
        {
            var random = Game.Random;
            Vector2 basePosition = Center;

            switch (_sourceEffect.Shape)
            {
                case EmissionShape.Point:
                    return basePosition;
                case EmissionShape.Circle:
                    {
                        float angle = (float)(random.NextDouble() * Math.PI * 2);
                        float distance = (float)(random.NextDouble() * _sourceEffect.EmissionRadius);
                        return basePosition + new Vector2(
                            (float)Math.Cos(angle) * distance,
                            (float)Math.Sin(angle) * distance
                        );
                    }
                case EmissionShape.Rectangle:
                    return basePosition + new Vector2(
                        (float)(random.NextDouble() - 0.5) * _sourceEffect.EmissionSize.X,
                        (float)(random.NextDouble() - 0.5) * _sourceEffect.EmissionSize.Y
                    );
                case EmissionShape.Line:
                    return basePosition + new Vector2(
                        (float)(random.NextDouble() - 0.5) * _sourceEffect.EmissionSize.X,
                        0
                    );
                case EmissionShape.Cone:
                    return basePosition;
                default:
                    return basePosition;
            }
        }

        private Vector2 GetParticleSpeed()
        {
            var random = Game.Random;
            Vector2 baseSpeed = _sourceEffect.ParticleSpeed; // ? Read live speed!
            Vector2 variation = _sourceEffect.ParticleSpeedVariation;

            Vector2 speed = new Vector2(
                baseSpeed.X + (float)(random.NextDouble() - 0.5) * variation.X * 2,
                baseSpeed.Y + (float)(random.NextDouble() - 0.5) * variation.Y * 2
            );

            if (_sourceEffect.Shape == EmissionShape.Cone)
            {
                // For cone emission, adjust speed direction
                float baseAngle = (float)Math.Atan2(_sourceEffect.EmissionDirection.Y, _sourceEffect.EmissionDirection.X);
                float angleVariation = _sourceEffect.ConeAngle * (float)Math.PI / 180f / 2f;
                float angle = baseAngle + (float)(random.NextDouble() - 0.5) * angleVariation * 2;
                
                float magnitude = speed.Length();
                speed = new Vector2(
                    (float)Math.Cos(angle) * magnitude,
                    (float)Math.Sin(angle) * magnitude
                );
            }

            return speed;
        }

        private float GetParticleLifetime()
        {
            var random = Game.Random;
            float variation = (float)(random.NextDouble() - 0.5) * _sourceEffect.ParticleLifetimeVariation * 2;
            return Math.Max(0.1f, _sourceEffect.ParticleLifetime + variation);
        }

        private float GetParticleStartSize()
        {
            var random = Game.Random;
            float variation = (float)(random.NextDouble() - 0.5) * _sourceEffect.ParticleSizeVariation * 2;
            return Math.Max(1f, _sourceEffect.ParticleStartSize + variation);
        }

        private float GetParticleEndSize()
        {
            var random = Game.Random;
            float variation = (float)(random.NextDouble() - 0.5) * _sourceEffect.ParticleSizeVariation * 2;
            return Math.Max(0f, _sourceEffect.ParticleEndSize + variation);
        }

        protected SpriteWrapper CreateDefaultParticleSprite()
        {
            // Return a simple white circle sprite
            return new SpriteWrapper((position, rotation, width, height, scale) =>
            {
                Raylib.DrawCircle((int)position.X, (int)position.Y, width / 2f, Color.White);
            });
        }

        public override void Draw()
        {
            // Emitters themselves don't draw anything visible
            // The particles handle their own drawing
        }

        // Public methods to control the emitter
        public void StartEmission() => _isEmitting = true;
        public void StopEmission() => _isEmitting = false;
        public void ClearParticles()
        {
            foreach (var particle in _particles)
            {
                particle.IsActive = false;
                _objectService.RemoveGameObject(particle);
            }
            _particles.Clear();
        }

        public void Burst(int particleCount)
        {
            for (int i = 0; i < particleCount && _particles.Count < _sourceEffect.MaxParticles; i++)
            {
                EmitParticle();
            }
        }


        /// <summary>
        /// Apply random color variation to a base color
        /// </summary>
        /// <param name="baseColor">The base color to modify</param>
        /// <param name="random">Random number generator instance</param>
        /// <returns>Color with random variation applied</returns>
        public Color ApplyColorVariation(Color baseColor, float colorVariation)
        {
            if (colorVariation <= 0f)
                return baseColor;

            // Generate random offsets for RGB components
            float rOffset = (float)(Game.Random.NextDouble() - 0.5) * colorVariation * 2f;
            float gOffset = (float)(Game.Random.NextDouble() - 0.5) * colorVariation * 2f;
            float bOffset = (float)(Game.Random.NextDouble() - 0.5) * colorVariation * 2f;

            // Apply offsets and clamp to valid color range
            int newR = Math.Clamp((int)(baseColor.R + rOffset), 0, 255);
            int newG = Math.Clamp((int)(baseColor.G + gOffset), 0, 255);
            int newB = Math.Clamp((int)(baseColor.B + bOffset), 0, 255);
            int newA = Math.Clamp((int)(baseColor.A + bOffset), 0, 255);

            return new Color(newR, newG, newB, newA); // Keep original alpha
        }
    }
}