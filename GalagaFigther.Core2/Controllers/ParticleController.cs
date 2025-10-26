using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface IParticleController : IController<ParticleInstance>
    {

    }
    public class ParticleController : IParticleController
    {
        public void Update(ParticleInstance particle, float frameTime)
        {
            UpdateColor(particle, frameTime);
            UpdateLifetime(particle, frameTime);
        }

        private void UpdateColor(ParticleInstance particle, float frameTime)
        {
            float t = 1f - (particle.CurrentLifetime / particle.Config.Lifetime);

            float currentSize = MathHelper.Lerp(particle.Config.StartSize, particle.Config.EndSize, t);
            particle.ScaleTo(currentSize, currentSize);
            particle.Color = ColorExtensions.Lerp(particle.StartColor, particle.EndColor, t);
        }

        private void UpdateLifetime(ParticleInstance particle, float frameTime)
        {
            if (particle.CurrentLifetime > 0)
                particle.CurrentLifetime -= frameTime;

            if (particle.CurrentLifetime < 0)
                particle.IsActive = false;
        }

        public void Draw(ParticleInstance gameObject, float frameTime)
        {
            gameObject.Sprite.Draw(gameObject);
        }
    }
}
