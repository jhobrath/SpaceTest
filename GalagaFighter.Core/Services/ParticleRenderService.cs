using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Particles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Services
{
    /// <summary>
    /// Service that renders particle effects based on ParticleEffect configurations in GameObjects
    /// Uses hash-based caching for optimal performance
    /// </summary>
    public interface IParticleRenderService
    {
        void RenderParticleEffects(GameObject gameObject, Game game);
        void ClearCacheForObject(Guid objectId);
    }

    public class ParticleRenderService : IParticleRenderService
    {
        private readonly IObjectService _objectService;
        private readonly Dictionary<string, ParticleEmitter> _emitterCache;

        public ParticleRenderService(IObjectService objectService)
        {
            _objectService = objectService;
            _emitterCache = new Dictionary<string, ParticleEmitter>();
        }

        public void RenderParticleEffects(GameObject gameObject, Game game)
        {
            // Look at the object's particle effects list - your vision!
            foreach (var effect in gameObject.ParticleEffects)
            {
                // Get or create emitter using hash-based caching
                var emitter = GetOrCreateEmitter(gameObject, effect);
                
                // Update emitter position based on object position + offset
                Vector2 worldPosition = CalculateWorldPosition(gameObject, effect);
                emitter.MoveTo(worldPosition.X, worldPosition.Y);
            }

            // Clean up emitters for effects no longer in the list
            CleanupUnusedEmitters(gameObject);
        }

        public void ClearCacheForObject(Guid objectId)
        {
            // Remove all cached emitters for this object when it's destroyed
            var keysToRemove = new List<string>();
            foreach (var key in _emitterCache.Keys)
            {
                if (key.StartsWith(objectId.ToString()))
                {
                    _emitterCache[key].IsActive = false;
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _emitterCache.Remove(key);
            }
        }

        private ParticleEmitter GetOrCreateEmitter(GameObject gameObject, ParticleEffect effect)
        {
            // Use the effect's hash for caching - your vision!
            string cacheKey = effect.GetCacheKey(gameObject.Id);

            if (_emitterCache.TryGetValue(cacheKey, out var existingEmitter) && existingEmitter.IsActive)
            {
                return existingEmitter;
            }

            // Create new emitter from the complete effect configuration
            var emitter = CreateEmitterFromEffect(gameObject, effect);
            _emitterCache[cacheKey] = emitter;
            return emitter;
        }

        private ParticleEmitter CreateEmitterFromEffect(GameObject gameObject, ParticleEffect effect)
        {
            Vector2 worldPosition = CalculateWorldPosition(gameObject, effect);

            // Convert ParticleEffect to ParticleEmitterConfig
            var config = new ParticleEmitterConfig
            {
                Shape = effect.Shape,
                EmissionRate = effect.EmissionRate,
                MaxParticles = effect.MaxParticles,
                EmissionRadius = effect.EmissionRadius,
                EmissionDirection = effect.EmissionDirection,
                ConeAngle = effect.ConeAngle,
                ParticleLifetime = effect.ParticleLifetime,
                ParticleLifetimeVariation = effect.ParticleLifetimeVariation,
                ParticleSpeed = effect.ParticleSpeed,
                ParticleSpeedVariation = effect.ParticleSpeedVariation,
                ParticleStartSize = effect.ParticleStartSize,
                ParticleEndSize = effect.ParticleEndSize,
                ParticleSizeVariation = effect.ParticleSizeVariation,
                ParticleStartColor = effect.ParticleStartColor,
                ParticleEndColor = effect.ParticleEndColor,
                Duration = effect.Duration,
                Loop = effect.Loop,
                EmitOnStart = effect.EmitOnStart,
                AutoDestroy = effect.AutoDestroy,
                UseGravity = effect.UseGravity,
                GravityStrength = effect.GravityStrength,
                ParticleDrag = effect.ParticleDrag
            };

            // Create sprite based on effect's sprite configuration
            SpriteWrapper sprite = null;
            if (effect.Sprites.Count > 0)
            {
                string selectedSprite = effect.SpriteSelection switch
                {
                    SpriteSelectionMode.First => effect.Sprites[0],
                    SpriteSelectionMode.Random => effect.Sprites[Game.Random.Next(effect.Sprites.Count)],
                    SpriteSelectionMode.Sequential => effect.Sprites[0], // Could implement cycling
                    _ => effect.Sprites[0]
                };

                // Create sprite wrapper for the selected sprite
                sprite = new SpriteWrapper($"Sprites/Particles/{selectedSprite}.png", effect.ParticleStartColor);
            }

            // Create emitter with the configuration and sprite
            var emitter = new ParticleEmitter(gameObject.Id, _objectService, worldPosition, config, sprite);
            _objectService.AddGameObject(emitter);
            
            return emitter;
        }

        private Vector2 CalculateWorldPosition(GameObject gameObject, ParticleEffect effect)
        {
            Vector2 offset = effect.Offset;

            if (effect.FollowRotation)
            {
                // Rotate offset based on game object rotation
                float radians = gameObject.Rotation * (float)Math.PI / 180f;
                float cos = (float)Math.Cos(radians);
                float sin = (float)Math.Sin(radians);
                
                offset = new Vector2(
                    effect.Offset.X * cos - effect.Offset.Y * sin,
                    effect.Offset.X * sin + effect.Offset.Y * cos
                );
            }

            return gameObject.Center + offset;
        }

        private void CleanupUnusedEmitters(GameObject gameObject)
        {
            // Get all current effect hashes for this object
            var currentEffectKeys = new HashSet<string>();
            foreach (var effect in gameObject.ParticleEffects)
            {
                string cacheKey = effect.GetCacheKey(gameObject.Id);
                currentEffectKeys.Add(cacheKey);
            }

            // Find and remove cached emitters not in current list
            var keysToRemove = new List<string>();
            foreach (var key in _emitterCache.Keys)
            {
                if (key.StartsWith(gameObject.Id.ToString()) && !currentEffectKeys.Contains(key))
                {
                    _emitterCache[key].IsActive = false;
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _emitterCache.Remove(key);
            }
        }
    }
}