using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.ParticleEmitters
{
    public interface IParticleEmissionSpawner
    {
        void Spawn(ParticleEmitter emitter, float frameTime);
        void Burst(ParticleEmitter emitter, int particleCount, Vector2 worldVelocity);
    }

    public class ParticleEmissionSpawner : IParticleEmissionSpawner
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IParticleCreationHandler _particleCreationHandler;
        private readonly IObjectService _objectService;

        public ParticleEmissionSpawner(IGameDataRegistry gameDataRegistry, IParticleCreationHandler particleCreationHandler, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _particleCreationHandler = particleCreationHandler;
            _objectService = objectService;
        }

        public void Spawn(ParticleEmitter emitter, float frameTime)
        {

            var emissionState = _gameDataRegistry.Get<ParticleEmissionState>(emitter);
            if (!emissionState.IsEmitting) return;
            
            emissionState.EmissionTimer += frameTime;
            
            if(emissionState.EmissionTimer < .05)
                return;


            var worldVelocity = emitter.Config.HasVerticalInertia ? GetWorldVelocity(emitter) : Vector2.Zero;

            float emissionInterval = 1f / emitter.Config.EmissionRate;
            
            while (emissionState.EmissionTimer >= emissionInterval)
            {
                EmitParticle(emitter, worldVelocity);
                emissionState.EmissionTimer -= emissionInterval;
            }
        }

        private Vector2 GetWorldVelocity(ParticleEmitter emitter)
        {
            var velocity = Vector2.Zero;
            var parent = (GameObject)emitter;
            while (parent.Owner != Game.Id)
            {
                parent = _objectService.Get(parent.Owner);
                velocity += new Vector2(0f, parent.Speed.Y);
            }

            return velocity;
        }

        public void Burst(ParticleEmitter emitter, int particleCount, Vector2 worldVelocity)
        {
            for (int i = 0; i < particleCount; i++)
            {
                EmitParticle(emitter, worldVelocity);
            }
        }

        private void EmitParticle(ParticleEmitter emitter, Vector2 worldVelocity)
        {
            _particleCreationHandler.CreateParticle(emitter, worldVelocity);
        }
    }
}