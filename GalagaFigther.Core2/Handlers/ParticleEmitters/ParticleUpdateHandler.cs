using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleUpdateHandler
    {
        void UpdateParticles(ParticleEmitter emitter, float frameTime);
        void CleanupInactiveParticles(ParticleEmitter emitter);
    }

    public class ParticleUpdateHandler : IParticleUpdateHandler
    {
        private readonly IObjectService _objectService;

        public ParticleUpdateHandler(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void UpdateParticles(ParticleEmitter emitter, float frameTime)
        {
            for (int i = 0; i < emitter.Particles.Count; i++)
            {
                var particle = emitter.Particles[i];
                if (particle.IsActive)
                {
                    UpdateParticleLifetime(particle, frameTime);
                    UpdateParticleVisuals(particle);
                }
            }
        }

        public void CleanupInactiveParticles(ParticleEmitter emitter)
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

        private void UpdateParticleLifetime(ParticleInstance particle, float frameTime)
        {
            // Only update particle lifetime - let GameObjectPositionService handle physics
            particle.CurrentLifetime -= frameTime;
            
            if (particle.CurrentLifetime <= 0f)
            {
                particle.IsActive = false;
            }
        }

        private void UpdateParticleVisuals(ParticleInstance particle)
        {
            if (!particle.IsActive) return;

            float t = 1f - (particle.CurrentLifetime / particle.MaxLifetime);
            
            float currentSize = MathHelper.Lerp(particle.StartSize, particle.EndSize, t);
            particle.ScaleTo(currentSize, currentSize);
            
            particle.Color = ColorExtensions.Lerp(particle.StartColor, particle.EndColor, t);
        }
    }
}