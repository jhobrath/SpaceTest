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
        private readonly IParticleDurationHandler _durationHandler;
        private readonly IParticleEmissionHandler _emissionHandler;
        private readonly IParticleUpdateHandler _updateHandler;
        private readonly IParticleDrawHandler _drawHandler;

        public ParticleEmitterController(
            IParticleDurationHandler durationHandler,
            IParticleEmissionHandler emissionHandler,
            IParticleUpdateHandler updateHandler,
            IParticleDrawHandler drawHandler)
        {
            _durationHandler = durationHandler;
            _emissionHandler = emissionHandler;
            _updateHandler = updateHandler;
            _drawHandler = drawHandler;
        }

        public void Update(ParticleEmitter emitter, float frameTime)
        {
            if (!emitter.IsActive) return;

            _durationHandler.UpdateDuration(emitter, frameTime);
            _emissionHandler.HandleEmission(emitter, frameTime);
            _updateHandler.UpdateParticles(emitter, frameTime);
            _updateHandler.CleanupInactiveParticles(emitter);
        }

        public void Draw(ParticleEmitter emitter, float frameTime)
        {
            _drawHandler.DrawParticles(emitter);
        }

        public void Burst(ParticleEmitter emitter, int particleCount)
        {
            _emissionHandler.Burst(emitter, particleCount);
        }
    }
}