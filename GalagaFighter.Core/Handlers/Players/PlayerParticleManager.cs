using GalagaFighter.Core.Models.Particles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerParticleManager
    {
        void Initialize(Player player);
        void UpdateMovementTrails(Player player, bool isMoving, float movementIntensity = 1f);
        void UpdateTrailPositions(Player player);
        void CleanupTrails();
    }

    public class PlayerParticleManager : IPlayerParticleManager
    {
        private readonly IParticleService _particleService;
        private ParticleEmitter _fireTrail;
        private ParticleEmitter _intenseFireTrail;
        private bool _isInitialized = false;
        private bool _wasMovingLastFrame = false;
        private float _currentIntensity = 0f;

        public PlayerParticleManager(IParticleService particleService)
        {
            _particleService = particleService;
        }

        public void Initialize(Player player)
        {
            if (_isInitialized) return;

            // Calculate initial trail position (behind the ship)
            Vector2 trailPosition = GetTrailPosition(player);

            // Create fire trail emitters
            _fireTrail = _particleService.CreateFireTrail(trailPosition, player.Id, player.IsPlayer1);
            _intenseFireTrail = _particleService.CreateIntenseFireTrail(trailPosition, player.Id, player.IsPlayer1);

            // Start with trails disabled
            _fireTrail.StopEmission();
            _intenseFireTrail.StopEmission();

            _isInitialized = true;
        }

        public void UpdateMovementTrails(Player player, bool isMoving, float movementIntensity = 1f)
        {
            if (!_isInitialized) return;

            // Smooth intensity changes
            float targetIntensity = isMoving ? movementIntensity : 0f;
            _currentIntensity = Lerp(_currentIntensity, targetIntensity, 0.1f);

            // Update trail positions
            UpdateTrailPositions(player);

            // Control trail emission based on movement intensity
            if (_currentIntensity > 0.1f)
            {
                if (_currentIntensity > 0.7f) // High intensity movement
                {
                    // Use intense fire trail for fast movement
                    _fireTrail.StopEmission();
                    _intenseFireTrail.StartEmission();
                }
                else // Normal movement
                {
                    // Use regular fire trail
                    _fireTrail.StartEmission();
                    _intenseFireTrail.StopEmission();
                }
            }
            else
            {
                // Stop both trails when not moving
                _fireTrail.StopEmission();
                _intenseFireTrail.StopEmission();
            }

            _wasMovingLastFrame = isMoving;
        }

        public void UpdateTrailPositions(Player player)
        {
            if (!_isInitialized) return;

            Vector2 trailPosition = GetTrailPosition(player);

            // Update emitter positions
            _fireTrail?.MoveTo(trailPosition.X, trailPosition.Y);
            _intenseFireTrail?.MoveTo(trailPosition.X, trailPosition.Y);
        }

        public void CleanupTrails()
        {
            _fireTrail?.StopEmission();
            _intenseFireTrail?.StopEmission();
            _isInitialized = false;
        }

        private Vector2 GetTrailPosition(Player player)
        {
            // Position the trail at the rear of the ship
            // For Player 1 (facing right), trail comes from the left side
            // For Player 2 (facing left), trail comes from the right side
            
            float offsetX = player.IsPlayer1 ? -player.Rect.Width * 0.4f : player.Rect.Width * 0.4f;
            float offsetY = 0f; // Center vertically

            return new Vector2(
                player.Center.X + offsetX,
                player.Center.Y + offsetY
            );
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }

    /// <summary>
    /// Extension methods to help integrate particle trails with existing player systems
    /// </summary>
    public static class PlayerParticleExtensions
    {
        /// <summary>
        /// Calculate movement intensity based on player speed and input duration
        /// </summary>
        public static float CalculateMovementIntensity(this Player player, float inputDuration)
        {
            // Base intensity on speed magnitude
            float speedIntensity = Math.Min(Math.Abs(player.Speed.Y) / 1200f, 1f);
            
            // Factor in input duration (longer held = more intense)
            float durationIntensity = Math.Min(inputDuration / 2f, 1f);
            
            return Math.Max(speedIntensity, durationIntensity);
        }

        /// <summary>
        /// Check if player is currently moving based on speed threshold
        /// </summary>
        public static bool IsMoving(this Player player)
        {
            return Math.Abs(player.Speed.Y) > 50f; // Threshold for "moving"
        }
    }
}