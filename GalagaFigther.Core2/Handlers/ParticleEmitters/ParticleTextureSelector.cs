using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleTextureSelector
    {
        string SelectTexture(ParticleEmitter emitter);
    }

    public class ParticleTextureSelector : IParticleTextureSelector
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly Random _random = new();

        public ParticleTextureSelector(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public string SelectTexture(ParticleEmitter emitter)
        {
            return emitter.Config.Textures[_random.Next(emitter.Config.Textures.Length)];
        }
    }
}