using GalagaFighter.Core2.Models.Particles;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleDrawHandler
    {
        void DrawParticles(ParticleEmitter emitter);
    }

    public class ParticleDrawHandler : IParticleDrawHandler
    {
        public void DrawParticles(ParticleEmitter emitter)
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