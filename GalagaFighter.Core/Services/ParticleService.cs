using GalagaFighter.Core.Models.Particles;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Services
{
    public interface IParticleService
    {
        ParticleEmitter CreateExplosion(Vector2 position, Guid owner, Raylib_cs.Color color = default, float intensity = 1f);
        ParticleEmitter CreateProjectileTrail(Vector2 position, Guid owner, Raylib_cs.Color color = default);
        ParticleEmitter CreateEngineExhaust(Vector2 position, Guid owner, bool isPlayer1);
        ParticleEmitter CreateImpact(Vector2 position, Vector2 impactDirection, Guid owner, Raylib_cs.Color color = default);
        ParticleEmitter CreateSparks(Vector2 position, Guid owner, int sparkCount = 10);
        ParticleEmitter CreateSmoke(Vector2 position, Guid owner, float duration = 2f);
        ParticleEmitter CreateMagicalEffect(Vector2 position, Guid owner, Raylib_cs.Color color = default);
        ParticleEmitter CreateIceCrystals(Vector2 position, Guid owner);
        ParticleEmitter CreateFireTrail(Vector2 position, Guid owner, bool isPlayer1);
        ParticleEmitter CreateIntenseFireTrail(Vector2 position, Guid owner, bool isPlayer1);
        ParticleEmitter CreateCustomEmitter(Vector2 position, Guid owner, ParticleEmitterConfig config, SpriteWrapper particleSprite = null);
        void UpdateParticleEmitters(Game game);
        int GetActiveParticleCount();
        void ClearAllParticles();
    }

    public class ParticleService : IParticleService
    {
        private readonly IObjectService _objectService;
        private readonly List<ParticleEmitter> _emitters;

        public ParticleService(IObjectService objectService)
        {
            _objectService = objectService;
            _emitters = new List<ParticleEmitter>();
        }

        public ParticleEmitter CreateExplosion(Vector2 position, Guid owner, Raylib_cs.Color color = default, float intensity = 1f)
        {
            var emitter = ParticleEffectFactory.CreateExplosion(_objectService, position, owner, color, intensity);
            
            // Use hard dot sprites for explosion - they look much more realistic than generated sprites
            var explosionSprite = SpriteGenerationService.CreateHardParticleSprite(color.Equals(default) ? Raylib_cs.Color.Orange : color);
            emitter.Sprite = explosionSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateProjectileTrail(Vector2 position, Guid owner, Raylib_cs.Color color = default)
        {
            var emitter = ParticleEffectFactory.CreateProjectileTrail(_objectService, position, owner, color);
            
            // Use medium dot sprites for trails - nice feathered edges
            var trailSprite = SpriteGenerationService.CreateMediumParticleSprite(color.Equals(default) ? Raylib_cs.Color.Yellow : color);
            emitter.Sprite = trailSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateEngineExhaust(Vector2 position, Guid owner, bool isPlayer1)
        {
            var emitter = ParticleEffectFactory.CreateEngineExhaust(_objectService, position, owner, isPlayer1);
            
            // Use soft to medium dot sprites for exhaust
            var exhaustColor = isPlayer1 ? Raylib_cs.Color.Blue : Raylib_cs.Color.Red;
            var exhaustSprite = SpriteGenerationService.CreateSoftParticleSprite(exhaustColor);
            emitter.Sprite = exhaustSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateImpact(Vector2 position, Vector2 impactDirection, Guid owner, Raylib_cs.Color color = default)
        {
            var emitter = ParticleEffectFactory.CreateImpact(_objectService, position, impactDirection, owner, color);
            
            // Use hard dot sprites for impact - sharp, defined particles
            var impactSprite = SpriteGenerationService.CreateHardParticleSprite(color.Equals(default) ? Raylib_cs.Color.White : color);
            emitter.Sprite = impactSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateSparks(Vector2 position, Guid owner, int sparkCount = 10)
        {
            var emitter = ParticleEffectFactory.CreateSparks(_objectService, position, owner, sparkCount);
            
            // Use hard dot sprites for sparks - they should be crisp and bright
            var sparkSprite = SpriteGenerationService.CreateHardParticleSprite(Raylib_cs.Color.Yellow);
            emitter.Sprite = sparkSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateSmoke(Vector2 position, Guid owner, float duration = 2f)
        {
            var emitter = ParticleEffectFactory.CreateSmoke(_objectService, position, owner, duration);
            
            // Use special smoke dot sprites with soft edges and gray colors
            var smokeSprite = SpriteGenerationService.CreateSmokeDotParticleSprite();
            emitter.Sprite = smokeSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateMagicalEffect(Vector2 position, Guid owner, Raylib_cs.Color color = default)
        {
            var emitter = ParticleEffectFactory.CreateMagicalEffect(_objectService, position, owner, color);
            
            // Use random intensity dots for magical sparkle effect
            var magicColor = color.Equals(default) ? Raylib_cs.Color.Magenta : color;
            var magicSprite = SpriteGenerationService.CreateRandomDotParticleSprite(magicColor);
            emitter.Sprite = magicSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateIceCrystals(Vector2 position, Guid owner)
        {
            var emitter = ParticleEffectFactory.CreateIceCrystals(_objectService, position, owner);
            
            // Use medium dot sprites for ice crystals - nice balance of sharp and soft
            var iceSprite = SpriteGenerationService.CreateMediumParticleSprite(Raylib_cs.Color.SkyBlue);
            emitter.Sprite = iceSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateFireTrail(Vector2 position, Guid owner, bool isPlayer1)
        {
            var emitter = ParticleEffectFactory.CreateFireTrail(_objectService, position, owner, isPlayer1);
            
            // Use special fire dot sprites with realistic fire colors and varied intensity
            var fireSprite = SpriteGenerationService.CreateFireDotParticleSprite();
            emitter.Sprite = fireSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateIntenseFireTrail(Vector2 position, Guid owner, bool isPlayer1)
        {
            var emitter = ParticleEffectFactory.CreateIntenseFireTrail(_objectService, position, owner, isPlayer1);
            
            // Use hotter fire colors and harder dots for intense fire
            var intenseFireSprite = SpriteGenerationService.CreateFireDotParticleSprite();
            emitter.Sprite = intenseFireSprite;
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public ParticleEmitter CreateCustomEmitter(Vector2 position, Guid owner, ParticleEmitterConfig config, SpriteWrapper particleSprite = null)
        {
            var emitter = new ParticleEmitter(owner, _objectService, position, config, particleSprite);
            
            _emitters.Add(emitter);
            _objectService.AddGameObject(emitter);
            return emitter;
        }

        public void UpdateParticleEmitters(Game game)
        {
            // Clean up inactive emitters
            for (int i = _emitters.Count - 1; i >= 0; i--)
            {
                if (!_emitters[i].IsActive)
                {
                    _emitters.RemoveAt(i);
                }
            }
        }

        public int GetActiveParticleCount()
        {
            int totalCount = 0;
            foreach (var emitter in _emitters)
            {
                if (emitter.IsActive)
                {
                    totalCount += emitter.ActiveParticleCount;
                }
            }
            return totalCount;
        }

        public void ClearAllParticles()
        {
            foreach (var emitter in _emitters)
            {
                emitter.ClearParticles();
                emitter.IsActive = false;
            }
            _emitters.Clear();
        }
    }
}