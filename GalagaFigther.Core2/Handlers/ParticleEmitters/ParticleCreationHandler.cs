using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleCreationHandler
    {
        void CreateParticle(ParticleEmitter emitter);
    }

    public class ParticleCreationHandler : IParticleCreationHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IParticleVelocityCalculator _velocityCalculator;
        private readonly IParticleTextureSelector _textureSelector;

        private static Random _random = new Random();

        public ParticleCreationHandler(
            IGameDataRegistry gameDataRegistry,
            IObjectService objectService,
            IParticleVelocityCalculator velocityCalculator,
            IParticleTextureSelector textureSelector)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _velocityCalculator = velocityCalculator;
            _textureSelector = textureSelector;
        }

        public void CreateParticle(ParticleEmitter emitter)
        {
            if (!emitter.Enabled)
                return;

            var velocity = _velocityCalculator.CalculateVelocity(emitter);
            float baseLifetime = emitter.Config.Lifetime;
            float variation = emitter.Config.LifetimeVariation;
            float actualLifetime = Math.Max(.05f, baseLifetime + ((float)_random.NextDouble() - 0.5f) * variation * 2f);
            float startSize = emitter.Config.StartSize;
            float endSize = startSize * 0.5f;
            
            string textureName = _textureSelector.SelectTexture(emitter);
            var sprite = new StillImageSprite($"Sprites/Particles/{textureName}.png");

            Vector2 emissionPosition;
            if (emitter.Config.EmitWithinParentBounds && emitter.Owner != Guid.Empty)
            {
                // Find parent GameObject
                var parent = _objectService.GetAll<GameObject>().FirstOrDefault(g => g.Id == emitter.Owner);
                if (parent != null && parent.Bounds != null && parent.Bounds.Length == 3)
                {
                    var vertices = PolygonVerticesCompiler.GetVertices(parent);
                    emissionPosition = RandomPointInPolygon(vertices, _random);
                }
                else
                {
                    // Fallback: use emitter.WorldPosition
                    emissionPosition = emitter.WorldPosition;
                }
            }
            else
            {
                var emissionOffset = new Vector2(
                    emitter.Config.EmissionRadius * (1f - 2f * (float)_random.NextDouble()),
                    emitter.Config.EmissionRadius * (1f - 2f * (float)_random.NextDouble())
                );
                emissionPosition = emitter.WorldPosition + emissionOffset;
            }

            var particle = new ParticleInstance(
                emitter.Id,
                emissionPosition,
                velocity,
                sprite,
                emitter.Config
            )
            {
                CurrentLifetime = actualLifetime
            };

            // Add particle to ObjectService so GameObjectPositionService can handle its physics
            _objectService.Add(particle);
        }

        private static Vector2 RotatePoint(Vector2 point, Vector2 center, float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            var translated = point - center;
            var rotated = new Vector2(
                translated.X * cos - translated.Y * sin,
                translated.X * sin + translated.Y * cos
            );
            return rotated + center;
        }

        private static Vector2 RandomPointInPolygon(Vector2[] poly, Random rand)
        {
            if (poly.Length == 3)
            {
                // Triangle: barycentric
                float u = (float)rand.NextDouble();
                float v = (float)rand.NextDouble();
                if (u + v > 1) { u = 1 - u; v = 1 - v; }
                return poly[0] * (1 - u - v) + poly[1] * u + poly[2] * v;
            }
            else if (poly.Length == 4)
            {
                // Quad: split into two triangles
                if (rand.NextDouble() < 0.5)
                {
                    // triangle 0,1,2
                    float u = (float)rand.NextDouble();
                    float v = (float)rand.NextDouble();
                    if (u + v > 1) { u = 1 - u; v = 1 - v; }
                    return poly[0] * (1 - u - v) + poly[1] * u + poly[2] * v;
                }
                else
                {
                    // triangle 2,3,0
                    float u = (float)rand.NextDouble();
                    float v = (float)rand.NextDouble();
                    if (u + v > 1) { u = 1 - u; v = 1 - v; }
                    return poly[2] * (1 - u - v) + poly[3] * u + poly[0] * v;
                }
            }
            else
            {
                // Fallback: just use first point
                return poly[0];
            }
        }
    }
}