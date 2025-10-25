using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleEmissionShepherd
    {
        void Herd(ParticleEmitter emitter, float frameTime);
    }

    public class ParticleUpdateHandler : IParticleEmissionShepherd
    {
        private readonly IObjectService _objectService;

        public ParticleUpdateHandler(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Herd(ParticleEmitter emitter, float frameTime)
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