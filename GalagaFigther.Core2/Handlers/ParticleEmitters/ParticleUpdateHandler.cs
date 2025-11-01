using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Game;
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
        private readonly IGameDataRegistry _gameDataRegistry;

        public ParticleUpdateHandler(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Herd(ParticleEmitter emitter, float frameTime)
        {
            //if (emitter.Owner == Game.Id)
            //    return;

            var screenData = _gameDataRegistry.Get<GameState>();

            if (emitter.WorldPosition.X < -50 || emitter.WorldPosition.X > screenData.ScreenSize.X + 50)
                emitter.IsActive = false;
            else if (emitter.WorldPosition.Y < -50 || emitter.WorldPosition.Y > screenData.ScreenSize.Y + 50)
                emitter.IsActive = false;
            //for (int i = 0; i < emitter.Particles.Count; i++)
            //{
            //    var particle = emitter.Particles[i];
            //    if (particle.IsActive)
            //    {
            //        UpdateParticleLifetime(particle, frameTime);
            //        UpdateParticleVisuals(particle);
            //    }
            //}
        }

        private void UpdateParticleLifetime(ParticleInstance particle, float frameTime)
        {
            //// Only update particle lifetime - let GameObjectPositionService handle physics
            //particle.CurrentLifetime -= frameTime;
            //
            //if (particle.CurrentLifetime <= 0f)
            //{
            //    particle.IsActive = false;
            //}
        }

        private void UpdateParticleVisuals(ParticleInstance particle)
        {
            //if (!particle.IsActive) return;

        }
    }
}