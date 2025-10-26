using GalagaFighter.Core2.Handlers.ParticleEmitters;
using GalagaFighter.Core2.Models.Particles;

namespace GalagaFighter.Core2.Controllers
{
    public interface IParticleEmitterController : IController<ParticleEmitter>
    {
    }

    public class ParticleEmitterController : IParticleEmitterController
    {
        private readonly IParticleEmissionTimer _particleEmissionTimer;
        private readonly IParticleEmissionSpawner _particleEmissionSpawner;
        private readonly IParticleEmissionShepherd _particleEmissionShepherd;

        public ParticleEmitterController(
            IParticleEmissionTimer particleEmissionTimer,
            IParticleEmissionSpawner particleEmissionSpawner,
            IParticleEmissionShepherd particleEmissionShepherd)
        {
            _particleEmissionTimer = particleEmissionTimer;
            _particleEmissionSpawner = particleEmissionSpawner;
            _particleEmissionShepherd = particleEmissionShepherd;
        }

        public void Update(ParticleEmitter emitter, float frameTime)
        {
            if (!emitter.IsActive) return;

            _particleEmissionTimer.Tick(emitter, frameTime);
            _particleEmissionSpawner.Spawn(emitter, frameTime);
            _particleEmissionShepherd.Herd(emitter, frameTime);
        }

        public void Draw(ParticleEmitter emitter, float frameTime)
        {
        }
    }
}