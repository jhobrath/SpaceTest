using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
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
            float lifetime = emitter.Config.Lifetime;
            float startSize = emitter.Config.StartSize;
            float endSize = startSize * 0.5f;
            
            string textureName = _textureSelector.SelectTexture(emitter);
            var sprite = new StillImageSprite($"Sprites/Particles/{textureName}.png");

            var emissionOffset = new Vector2(
                emitter.Config.EmissionRadius * (1f - 2f * (float)_random.NextDouble()),
                emitter.Config.EmissionRadius * (1f - 2f * (float)_random.NextDouble())
            );

            var particle = new ParticleInstance(
                emitter.Id,
                emitter.WorldPosition + emissionOffset,
                velocity,
                sprite,
                emitter.Config
            );

            // Add particle to ObjectService so GameObjectPositionService can handle its physics
            _objectService.Add(particle);
        }
    }
}