using GalagaFighter.Core.Models.Particles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Particles
{
    public enum EmissionShape
    {
        Point,      // Emit from a single point
        Circle,     // Emit from around a circle
        Rectangle,  // Emit from within a rectangle
        Line,       // Emit along a line
        Cone        // Emit in a cone direction
    }

    public class ParticleEmitterConfig
    {
        // Emission properties
        public EmissionShape Shape { get; set; } = EmissionShape.Point;
        public float EmissionRate { get; set; } = 10f; // Particles per second
        public int MaxParticles { get; set; } = 100;
        public float EmissionRadius { get; set; } = 10f; // For circle/cone shapes
        public Vector2 EmissionSize { get; set; } = new Vector2(20f, 20f); // For rectangle shape
        public float ConeAngle { get; set; } = 45f; // For cone shape (degrees)
        public Vector2 EmissionDirection { get; set; } = new Vector2(0, -1); // For cone shape

        // Particle properties
        public float ParticleLifetime { get; set; } = 2f;
        public float ParticleLifetimeVariation { get; set; } = 0.5f;
        public Vector2 ParticleSpeed { get; set; } = new Vector2(50f, 50f);
        public Vector2 ParticleSpeedVariation { get; set; } = new Vector2(25f, 25f);
        public Vector2 ParticleAcceleration { get; set; } = Vector2.Zero;
        public float ParticleStartSize { get; set; } = 5f;
        public float ParticleEndSize { get; set; } = 1f;
        public float ParticleSizeVariation { get; set; } = 2f;
        public Color ParticleStartColor { get; set; } = Color.White;
        public Color ParticleEndColor { get; set; } = Color.Blank;
        public float ParticleDrag { get; set; } = 0f;
        public bool UseGravity { get; set; } = false;
        public float GravityStrength { get; set; } = 98f;

        // Emitter behavior
        public float Duration { get; set; } = -1f; // -1 for infinite
        public bool Loop { get; set; } = true;
        public bool EmitOnStart { get; set; } = true;
        public bool AutoDestroy { get; set; } = true; // Destroy emitter when done
    }

    public class ParticleEmitter : GameObject
    {
        private readonly IObjectService _objectService;
        private readonly ParticleEmitterConfig _config;
        private readonly List<Particle> _particles;
        private float _emissionTimer;
        private float _durationTimer;
        private bool _isEmitting;

        public ParticleEmitterConfig Config => _config;
        public int ActiveParticleCount => _particles.Count;
        public bool IsEmitting => _isEmitting;

        public ParticleEmitter(Guid owner, IObjectService objectService, Vector2 position, 
                              ParticleEmitterConfig config, SpriteWrapper particleSprite = null) 
            : base(owner, particleSprite, position, Vector2.One, Vector2.Zero)
        {
            _objectService = objectService;
            _config = config ?? new ParticleEmitterConfig();
            _particles = new List<Particle>();
            _emissionTimer = 0f;
            _durationTimer = 0f;
            _isEmitting = _config.EmitOnStart;
            SetDrawPriority(0.4); // Draw emitter before particles
        }

        public override void Update(Game game)
        {
            float frameTime = Raylib.GetFrameTime();

            // Update duration if not infinite
            if (_config.Duration > 0)
            {
                _durationTimer += frameTime;
                if (_durationTimer >= _config.Duration)
                {
                    if (_config.Loop)
                    {
                        _durationTimer = 0f;
                    }
                    else
                    {
                        _isEmitting = false;
                        if (_config.AutoDestroy && _particles.Count == 0)
                        {
                            IsActive = false;
                            return;
                        }
                    }
                }
            }

            // Emit particles
            if (_isEmitting)
            {
                _emissionTimer += frameTime;
                float emissionInterval = 1f / _config.EmissionRate;
                
                while (_emissionTimer >= emissionInterval && _particles.Count < _config.MaxParticles)
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

            var particleSprite = Sprite ?? CreateDefaultParticleSprite();
            
            var particle = new Particle(
                Id, // Emitter is the owner
                particleSprite,
                emissionPosition,
                new Vector2(startSize, startSize),
                particleSpeed,
                lifetime,
                _config.ParticleStartColor,
                _config.ParticleEndColor,
                startSize,
                endSize
            )
            {
                Acceleration = _config.ParticleAcceleration,
                UseGravity = _config.UseGravity,
                GravityStrength = _config.GravityStrength,
                Drag = _config.ParticleDrag
            };

            _particles.Add(particle);
            _objectService.AddGameObject(particle);
        }

        private Vector2 GetEmissionPosition()
        {
            var random = Game.Random;
            Vector2 basePosition = Center;

            switch (_config.Shape)
            {
                case EmissionShape.Point:
                    return basePosition;
                case EmissionShape.Circle:
                    {
                        float angle = (float)(random.NextDouble() * Math.PI * 2);
                        float distance = (float)(random.NextDouble() * _config.EmissionRadius);
                        return basePosition + new Vector2(
                            (float)Math.Cos(angle) * distance,
                            (float)Math.Sin(angle) * distance
                        );
                    }
                case EmissionShape.Rectangle:
                    return basePosition + new Vector2(
                        (float)(random.NextDouble() - 0.5) * _config.EmissionSize.X,
                        (float)(random.NextDouble() - 0.5) * _config.EmissionSize.Y
                    );
                case EmissionShape.Line:
                    return basePosition + new Vector2(
                        (float)(random.NextDouble() - 0.5) * _config.EmissionSize.X,
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
            Vector2 baseSpeed = _config.ParticleSpeed;
            Vector2 variation = _config.ParticleSpeedVariation;

            Vector2 speed = new Vector2(
                baseSpeed.X + (float)(random.NextDouble() - 0.5) * variation.X * 2,
                baseSpeed.Y + (float)(random.NextDouble() - 0.5) * variation.Y * 2
            );

            if (_config.Shape == EmissionShape.Cone)
            {
                // For cone emission, adjust speed direction
                float baseAngle = (float)Math.Atan2(_config.EmissionDirection.Y, _config.EmissionDirection.X);
                float angleVariation = _config.ConeAngle * (float)Math.PI / 180f / 2f;
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
            float variation = (float)(random.NextDouble() - 0.5) * _config.ParticleLifetimeVariation * 2;
            return Math.Max(0.1f, _config.ParticleLifetime + variation);
        }

        private float GetParticleStartSize()
        {
            var random = Game.Random;
            float variation = (float)(random.NextDouble() - 0.5) * _config.ParticleSizeVariation * 2;
            return Math.Max(1f, _config.ParticleStartSize + variation);
        }

        private float GetParticleEndSize()
        {
            var random = Game.Random;
            float variation = (float)(random.NextDouble() - 0.5) * _config.ParticleSizeVariation * 2;
            return Math.Max(0f, _config.ParticleEndSize + variation);
        }

        private SpriteWrapper CreateDefaultParticleSprite()
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
            for (int i = 0; i < particleCount && _particles.Count < _config.MaxParticles; i++)
            {
                EmitParticle();
            }
        }
    }
}