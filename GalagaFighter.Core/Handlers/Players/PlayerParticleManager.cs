using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerParticleManager
    {
        void UpdateMovementTrails(Player player);
        void CleanupTrails(Player player);
        void UpdateModifierEffects(Player player, EffectModifiers modifiers);
    }

    public class PlayerParticleManager : IPlayerParticleManager
    {
        private float _currentIntensity = 0f;

        private readonly IParticleRenderService _particleRenderService;

        public PlayerParticleManager(IParticleRenderService particleRenderService)
        {
            _particleRenderService = particleRenderService;
        }

        public void UpdateMovementTrails(Player player)
        {
            // Calculate intensity directly from player speed
            float targetIntensity = Math.Min(Math.Abs(player.Speed.Y) / 1200f, 1f);
            
            // Smooth intensity changes
            _currentIntensity = Lerp(_currentIntensity, targetIntensity, 0.1f);

            // Update trail effects based on movement intensity
            if (_currentIntensity > 0.1f)
            {
                if (_currentIntensity > 0.7f) // High intensity movement
                {
                    // Remove normal fire trail, add intense fire trail
                    RemoveTrailEffect(player, "FireTrail");
                    AddIntenseFireTrail(player);
                }
                else // Normal movement
                {
                    // Remove intense fire trail, add normal fire trail
                    RemoveTrailEffect(player, "IntenseFireTrail");
                    AddNormalFireTrail(player);
                }
            }
            else
            {
                // Remove both trails when not moving (intensity is 0 or very low)
                RemoveTrailEffect(player, "FireTrail");
                RemoveTrailEffect(player, "IntenseFireTrail");
            }
        }

        public void CleanupTrails(Player player)
        {
            // Remove all trail effects
            RemoveTrailEffect(player, "FireTrail");
            RemoveTrailEffect(player, "IntenseFireTrail");
        }

        private void AddNormalFireTrail(Player player)
        {
            // Check if already has this effect
            if (player.ParticleEffects.Any(e => e.Name == "FireTrail"))
                return;

            var fireTrail = ParticleEffectsLibrary.Get("FireTrail");
            fireTrail.Offset = GetTrailOffset(player);
            // FIXED: Use proper fire colors (red/orange) instead of blue
            if (player.IsPlayer1)
            {
                fireTrail.ParticleStartColor = Color.Orange;
                fireTrail.ParticleEndColor = new Color(255, 50, 0, 0); // Orange to red fade
            }
            else
            {
                fireTrail.ParticleStartColor = Color.Red;
                fireTrail.ParticleEndColor = new Color(255, 100, 0, 0); // Red to orange fade
            }
            player.ParticleEffects.Add(fireTrail);
        }

        private void AddIntenseFireTrail(Player player)
        {
            // Check if already has this effect
            if (player.ParticleEffects.Any(e => e.Name == "IntenseFireTrail"))
                return;

            var intenseFireTrail = ParticleEffectsLibrary.Get("IntenseFireTrail");
            intenseFireTrail.Offset = GetTrailOffset(player);
            // FIXED: Use proper intense fire colors (yellow/orange) instead of blue
            if (player.IsPlayer1)
            {
                intenseFireTrail.ParticleStartColor = Color.Yellow; // Hot fire is yellow
                intenseFireTrail.ParticleEndColor = new Color(255, 165, 0, 0); // Yellow to orange fade
            }
            else
            {
                intenseFireTrail.ParticleStartColor = new Color(255, 200, 0, 255); // Bright yellow-orange
                intenseFireTrail.ParticleEndColor = new Color(255, 0, 0, 0); // Fade to red
            }
            player.ParticleEffects.Add(intenseFireTrail);
        }

        private void RemoveTrailEffect(Player player, string effectName)
        {
            player.ParticleEffects.RemoveAll(e => e.Name == effectName);
        }

        private Vector2 GetTrailOffset(Player player)
        {
            // Position the trail at the rear of the ship
            // Set offset for 0 degree rotation (facing upwards)
            float offsetX =  0f;
            float offsetY = player.Rect.Height * 0.4f;

            return new Vector2(offsetX, offsetY);
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public void UpdateModifierEffects(Player player, EffectModifiers modifiers)
        {
            _particleRenderService.RenderParticleEffectFromModifiers(player, modifiers);
        }
    }
}