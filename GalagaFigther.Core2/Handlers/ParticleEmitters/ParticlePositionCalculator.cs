using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticlePositionCalculator
    {
        Vector2 CalculatePosition(ParticleEmitter emitter);
    }

    public class ParticlePositionCalculator : IParticlePositionCalculator
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public ParticlePositionCalculator(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public Vector2 CalculatePosition(ParticleEmitter emitter)
        {
            var config = _gameDataRegistry.Get<ParticleEffectConfig>(emitter);
            var finalOffset = emitter.Offset;
            
            if (config.FollowRotation)
            {
                float rotationRadians = emitter.WorldRotation * MathF.PI / 180f;
                var rotatedOffset = new Vector2(
                    finalOffset.X * MathF.Cos(rotationRadians) - finalOffset.Y * MathF.Sin(rotationRadians),
                    finalOffset.X * MathF.Sin(rotationRadians) + finalOffset.Y * MathF.Cos(rotationRadians)
                );
                return emitter.WorldPosition + rotatedOffset;
            }
            
            return emitter.WorldPosition + finalOffset;
        }
    }
}