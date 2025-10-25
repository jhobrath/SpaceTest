using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleEmissionState : IGameObjectData<ParticleEmitter>
    {
        public float EmissionTimer { get; set; } = 0f;
        public float DurationTimer { get; set; } = 0f;
        public bool IsEmitting { get; set; } = true;
    }
}