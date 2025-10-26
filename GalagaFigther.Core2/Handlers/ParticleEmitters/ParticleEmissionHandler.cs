using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleEmissionSpawner
    {
        void Spawn(ParticleEmitter emitter, float frameTime);
        void Burst(ParticleEmitter emitter, int particleCount);
    }

    public class ParticleEmissionHandler : IParticleEmissionSpawner
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IParticleCreationHandler _particleCreationHandler;

        public ParticleEmissionHandler(IGameDataRegistry gameDataRegistry, IParticleCreationHandler particleCreationHandler)
        {
            _gameDataRegistry = gameDataRegistry;
            _particleCreationHandler = particleCreationHandler;
        }

        public void Spawn(ParticleEmitter emitter, float frameTime)
        {
            var emissionState = _gameDataRegistry.Get<ParticleEmissionState>(emitter);
            if (!emissionState.IsEmitting) return;

            emissionState.EmissionTimer += frameTime;
            
            float emissionInterval = 1f / emitter.Config.BaseEmissionRate;
            
            while (emissionState.EmissionTimer >= emissionInterval)
            {
                EmitParticle(emitter);
                emissionState.EmissionTimer -= emissionInterval;
            }
        }

        public void Burst(ParticleEmitter emitter, int particleCount)
        {
            for (int i = 0; i < particleCount; i++)
            {
                EmitParticle(emitter);
            }
        }

        private void EmitParticle(ParticleEmitter emitter)
        {
            var particle = _particleCreationHandler.CreateParticle(emitter);
            emitter.Particles.Add(particle);
        }
    }
}