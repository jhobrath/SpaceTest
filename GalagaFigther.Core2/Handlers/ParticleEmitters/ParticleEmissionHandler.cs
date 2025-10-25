using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleEmissionHandler
    {
        void HandleEmission(ParticleEmitter emitter, float frameTime);
        void EmitParticle(ParticleEmitter emitter);
        void Burst(ParticleEmitter emitter, int particleCount);
    }

    public class ParticleEmissionHandler : IParticleEmissionHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IParticleCreationHandler _particleCreationHandler;

        public ParticleEmissionHandler(IGameDataRegistry gameDataRegistry, IParticleCreationHandler particleCreationHandler)
        {
            _gameDataRegistry = gameDataRegistry;
            _particleCreationHandler = particleCreationHandler;
        }

        public void HandleEmission(ParticleEmitter emitter, float frameTime)
        {
            var emissionState = _gameDataRegistry.Get<ParticleEmissionState>(emitter);
            if (!emissionState.IsEmitting) return;

            var config = _gameDataRegistry.Get<ParticleEffectConfig>(emitter);
            emissionState.EmissionTimer += frameTime;
            
            float emissionInterval = 1f / config.BaseEmissionRate;
            
            while (emissionState.EmissionTimer >= emissionInterval)
            {
                EmitParticle(emitter);
                emissionState.EmissionTimer -= emissionInterval;
            }
        }

        public void EmitParticle(ParticleEmitter emitter)
        {
            var particle = _particleCreationHandler.CreateParticle(emitter);
            emitter.Particles.Add(particle);
        }

        public void Burst(ParticleEmitter emitter, int particleCount)
        {
            for (int i = 0; i < particleCount; i++)
            {
                EmitParticle(emitter);
            }
        }
    }
}