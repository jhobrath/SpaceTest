using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleEmissionCleaner
    {
        void Clean(ParticleEmitter emitter);
        void Draw(ParticleEmitter emitter);
    }

    public class ParticleDrawHandler : IParticleEmissionCleaner
    {
        private readonly IObjectService _objectService;

        public ParticleDrawHandler(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Clean(ParticleEmitter emitter)
        {
            for (int i = emitter.Particles.Count - 1; i >= 0; i--)
            {
                var particle = emitter.Particles[i];
                if (!particle.IsActive)
                {
                    // Remove from ObjectService so it's no longer processed by GameObjectPositionService
                    _objectService.Remove(particle);
                    emitter.Particles.RemoveAt(i);
                }
            }
        }

        public void Draw(ParticleEmitter emitter)
        {
            foreach (var particle in emitter.Particles)
            {
                if (particle.IsActive)
                {
                    particle.Sprite.Draw(particle);
                }
            }
        }
    }
}