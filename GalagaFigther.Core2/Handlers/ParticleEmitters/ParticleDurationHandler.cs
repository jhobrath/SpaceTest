using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleEmissionTimer
    {
        void Tick(ParticleEmitter emitter, float frameTime);
    }

    public class ParticleDurationHandler : IParticleEmissionTimer
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public ParticleDurationHandler(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Tick(ParticleEmitter emitter, float frameTime)
        {
            var emissionState = _gameDataRegistry.Get<ParticleEmissionState>(emitter);
            
            if (emitter.Config.Duration > 0f)
            {
                emissionState.DurationTimer += frameTime;
                if (emissionState.DurationTimer >= emitter.Config.Duration)
                {
                    if (emitter.Config.Loop)
                    {
                        emissionState.DurationTimer = 0f;
                    }
                    else
                    {
                        emissionState.IsEmitting = false;
                        
                        // Check if all particles are done too
                        if (emitter.Particles.Count == 0)
                        {
                            emitter.IsActive = false;
                        }
                    }
                }
            }
        }
    }
}