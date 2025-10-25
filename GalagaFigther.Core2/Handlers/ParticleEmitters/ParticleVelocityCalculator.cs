using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleVelocityCalculator
    {
        Vector2 CalculateVelocity(ParticleEmitter emitter);
    }

    public class ParticleVelocityCalculator : IParticleVelocityCalculator
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly Random _random = new();

        public ParticleVelocityCalculator(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public Vector2 CalculateVelocity(ParticleEmitter emitter)
        {
            var config = _gameDataRegistry.Get<ParticleEffectConfig>(emitter);
            float speed = config.BaseSpeed;
            float angle = (float)(_random.NextDouble() * Math.PI * 2);
            
            if (config.FollowRotation)
            {
                angle += emitter.WorldRotation * MathF.PI / 180f;
            }
            
            return new Vector2(
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed
            );
        }
    }
}