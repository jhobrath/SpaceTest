using GalagaFighter.Core2.Handlers.ParticleEmitters;
using GalagaFighter.Core2.Models.Particles;

namespace GalagaFighter.Core2.Controllers
{
    public interface IParticleEmitterController : IController<ParticleEmitter>
    {
        void Burst(ParticleEmitter emitter, int particleCount);
    }

    public class ParticleEmitterController : IParticleEmitterController
    {
        private readonly IParticleEmissionTimer _particleEmissionTimer;
        private readonly IParticleEmissionSpawner _particleEmissionSpawner;
        private readonly IParticleEmissionShepherd _particleEmissionShepherd;
        private readonly IParticleEmissionCleaner _particleEmissionCleaner;

        public ParticleEmitterController(
            IParticleEmissionTimer particleEmissionTimer,
            IParticleEmissionSpawner particleEmissionSpawner,
            IParticleEmissionShepherd particleEmissionShepherd,
            IParticleEmissionCleaner particleEmissionCleaner)
        {
            _particleEmissionTimer = particleEmissionTimer;
            _particleEmissionSpawner = particleEmissionSpawner;
            _particleEmissionShepherd = particleEmissionShepherd;
            _particleEmissionCleaner = particleEmissionCleaner;
        }

        public void Update(ParticleEmitter emitter, float frameTime)
        {
            if (!emitter.IsActive) return;

            _particleEmissionTimer.Tick(emitter, frameTime);
            _particleEmissionSpawner.Spawn(emitter, frameTime);
            _particleEmissionShepherd.Herd(emitter, frameTime);
            _particleEmissionCleaner.Clean(emitter);
        }

        public void Draw(ParticleEmitter emitter, float frameTime)
        {
            _particleEmissionCleaner.Draw(emitter);
        }

        public void Burst(ParticleEmitter emitter, int particleCount)
        {
            _particleEmissionSpawner.Burst(emitter, particleCount);
        }
    }
}