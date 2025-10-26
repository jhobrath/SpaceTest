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

        private static readonly Random _random = new Random();

        public ParticleVelocityCalculator(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public Vector2 CalculateVelocity(ParticleEmitter emitter)
        {
            //var config = _gameDataRegistry.Get<ParticleEffectConfig>(emitter);
            
            // Start with the base speed vector
            Vector2 velocity = emitter.Config.Speed;



            var xVariation = emitter.Config.SpeedVariation.X - (float)_random.NextDouble() * emitter.Config.SpeedVariation.X * 2f;
            var yVariation = emitter.Config.SpeedVariation.Y - (float)_random.NextDouble() * emitter.Config.SpeedVariation.Y * 2f;
            velocity = velocity + new Vector2(xVariation, yVariation);



            // Apply emitter rotation
            float emitterAngle = emitter.WorldRotation * MathF.PI / 180f;
            float cos = (float)Math.Cos(emitterAngle);
            float sin = (float)Math.Sin(emitterAngle);
            
            velocity = new Vector2(
                velocity.X * cos - velocity.Y * sin,
                velocity.X * sin + velocity.Y * cos
            );
            
            return velocity;
        }
    }
}