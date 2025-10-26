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
        ParticleInstance CreateParticle(ParticleEmitter emitter);
    }

    public class ParticleCreationHandler : IParticleCreationHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IParticleVelocityCalculator _velocityCalculator;
        private readonly IParticleTextureSelector _textureSelector;

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

        public ParticleInstance CreateParticle(ParticleEmitter emitter)
        {
            var velocity = _velocityCalculator.CalculateVelocity(emitter);
            float lifetime = emitter.Config.BaseLifetime;
            float startSize = emitter.Config.BaseSize;
            float endSize = startSize * 0.5f;
            
            string textureName = _textureSelector.SelectTexture(emitter);
            var sprite = new StillImageSprite($"Sprites/Particles/{textureName}.png");
            
            var particle = new ParticleInstance(
                emitter.Id,
                emitter.WorldPosition,
                new Vector2(startSize, startSize),
                velocity,
                sprite,
                lifetime,
                startSize,
                endSize,
                Color.White,
                new Color(255, 255, 255, 0)
            );

            // Add particle to ObjectService so GameObjectPositionService can handle its physics
            _objectService.Add(particle);
            
            return particle;
        }
    }
}