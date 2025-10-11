using GalagaFighter.Core.CPU.Gambits;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.CPU
{
    public interface ICpuDecisionMaker2 : ICpuDecisionMaker
    {
        void InitializePlayer(Player player);
    }

    public class CpuDecisionMaker2 : ICpuDecisionMaker
    {
        private bool _moveDown = false;
        private bool _moveUp = false;
        private bool _switch = false;
        private bool _deploy = false;
        private bool _shoot = false;

        private readonly IOpponentBulletWatcher _opponentBulletWatcher;
        private readonly IObjectService _objectService;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private Player? _player;

        private bool _deployWasReleased = true;
        private float _shootRefillWait = 2f;
        
        // Switch/Parry timing tracking
        private float _lastSwitchTime = 0f; // Track when we last used switch
        private const float SWITCH_COOLDOWN = 2.5f; // Minimum time between switch uses (2-3 seconds as requested)

        public CpuDecisionMaker2(IOpponentBulletWatcher opponentBulletWatcher,
            IObjectService objectService, IPlayerManagerFactory playerManagerFactory)
        {
            _opponentBulletWatcher = opponentBulletWatcher;
            _objectService = objectService;
            _playerManagerFactory = playerManagerFactory;
        }

        public void InitializePlayer(Player player)
        {
            _player = player;
        }

        public bool IsDeployDown()
        {
            return false;
        }

        public bool IsMoveLeftDown()
        {
            return _moveDown;
        }

        public bool IsMoveRightDown()
        {
            return _moveUp;
        }

        public bool IsShootDown()
        {
            return true;
        }

        public bool IsSwitchDown()
        {
            return _switch;
        }

        public void Update()
        {
            if (_player == null)
                return;

            if (Game.Random.NextDouble() > .25f)
                return;
            
            _moveDown = false;
            _moveUp = false;
            _switch = false;
            _deploy = false;
            _shoot = false;

            var modifiers = GetModifiers();
            
            // Handle emergency parry first - this takes priority over everything
            HandleEmergencyParry();
            
            if(!modifiers.Untouchable)
                HandleProjectileThreats();

            HandlePhaseShiftDeploy(modifiers);
            HandlePlayerChasing();
            HandleShooting();
            
            _deployWasReleased = !_deploy;
        }

        private void HandleShooting()
        {
            _shootRefillWait += Raylib.GetFrameTime();

            var resourceManager = _playerManagerFactory.GetResourceManager(_player!);
            if (resourceManager.ShootMeter < .5f)
            {
                _shootRefillWait = 0f;
                return;
            }

            if (_shootRefillWait > 2f)
            {
                _shoot = true;
            }
        }

        private void HandleEmergencyParry()
        {
            var currentTime = (float)Raylib.GetTime();
            
            // Don't use switch if we used it too recently (cooldown management)
            if (currentTime - _lastSwitchTime < SWITCH_COOLDOWN)
                return;

            var resourceManager = _playerManagerFactory.GetResourceManager(_player!);
            
            // Need sufficient shield meter to use parry (similar to original CPU logic)
            if (resourceManager.ShieldMeter < 30f)
                return;

            // Get opponent projectiles to check for imminent threats
            var opponent = GetOpponent();
            var opponentProjectiles = _objectService.GetGameObjects<Projectile>()
                .Where(p => p.Owner == opponent.Id)
                .ToList();

            var playerCenter = _player.Center;
            var playerHeight = _player.Rect.Height;

            // Check for imminent threats (within 50 pixels and < 0.1 seconds away)
            foreach (var projectile in opponentProjectiles)
            {
                var distanceToPlayer = Math.Abs(projectile.Center.Y - playerCenter.Y);
                
                // Check if projectile is close enough to be considered a threat
                if (distanceToPlayer > 50f)
                    continue;

                // Estimate time to impact based on projectile speed and horizontal distance
                var horizontalDistance = Math.Abs(projectile.Center.X - playerCenter.X);
                var timeToImpact = horizontalDistance / Math.Max(Math.Abs(projectile.Speed.X), 1f);

                // If threat is imminent (< 0.1 seconds away), use emergency parry
                if (timeToImpact < 0.1f)
                {
                    _switch = true;
                    _lastSwitchTime = (float)currentTime;
                    return; // Exit early since we found a critical threat
                }
            }
        }

        private void HandleProjectileThreats()
        {
            var threats = _opponentBulletWatcher.GetDestinies(_player);
            if (!threats.Any())
                return;

            var playerY = _player.Center.Y;
            var playerHeight = _player.Rect.Height;
            var opponent = GetOpponent();
            var opponentY = opponent.Center.Y;

            // Define a "safe" distance to keep from projectiles.
            var safetyBuffer = playerHeight * 0.8f; // Reduced buffer to stay closer to action

            // Filter for imminent threats - be much more conservative about what we consider a threat
            var imminentThreats = threats
                .Where(t => t.TimeLeft < 0.5f && Math.Abs(t.TargetY - playerY) < playerHeight * 1.5f) // More restrictive threat detection
                .OrderBy(t => t.TimeLeft)
                .ToList();

            if (!imminentThreats.Any())
                return;

            // Find safe zones. A safe zone is a vertical corridor free of projectile impacts.
            var sortedThreatsY = imminentThreats.Select(t => t.TargetY).OrderBy(y => y).ToList();

            var safeZones = new List<(float start, float end, float size, float score)>();
            float lastThreatY = 0;

            foreach (var threatY in sortedThreatsY)
            {
                var zoneStart = lastThreatY + safetyBuffer;
                var zoneEnd = threatY - safetyBuffer;
                if (zoneEnd > zoneStart)
                {
                    var zoneCenterY = (zoneStart + zoneEnd) / 2;
                    var zoneDistanceToOpponent = Math.Abs(zoneCenterY - opponentY);
                    
                    // Score based on zone size and proximity to opponent (lower distance = higher score)
                    var sizeScore = zoneEnd - zoneStart;
                    var proximityScore = Math.Max(0, 400 - zoneDistanceToOpponent) / 400f * 200f; // Heavily weight opponent proximity
                    var totalScore = sizeScore + proximityScore;
                    
                    safeZones.Add((zoneStart, zoneEnd, zoneEnd - zoneStart, totalScore));
                }
                lastThreatY = threatY;
            }

            // Add the final safe zone from the last threat to the bottom of the screen.
            if (lastThreatY < Game.Height - safetyBuffer)
            {
                var zoneStart = lastThreatY + safetyBuffer;
                var zoneEnd = Game.Height;
                var zoneCenterY = (zoneStart + zoneEnd) / 2;
                var finalZoneDistanceToOpponent = Math.Abs(zoneCenterY - opponentY);
                
                var sizeScore = zoneEnd - zoneStart;
                var proximityScore = Math.Max(0, 400 - finalZoneDistanceToOpponent) / 400f * 200f;
                var totalScore = sizeScore + proximityScore;
                
                safeZones.Add((zoneStart, zoneEnd, zoneEnd - zoneStart, totalScore));
            }

            // If there are no safe zones, check if we can make a minimal dodge
            if (!safeZones.Any())
            {
                var mostImminentThreat = imminentThreats.OrderBy(t => t.TimeLeft).First();
                
                // Only dodge if the threat is very close and directly hitting us
                if (mostImminentThreat.TimeLeft < 0.2f && Math.Abs(mostImminentThreat.TargetY - playerY) < playerHeight * 0.8f)
                {
                    if (mostImminentThreat.TargetY > playerY)
                        _moveUp = true; // Move up to avoid threat from below
                    else
                        _moveDown = true; // Move down to avoid threat from above
                }
                return;
            }

            // Find the best safe zone based on our scoring system
            var bestZone = safeZones.OrderByDescending(z => z.score).First();
            var targetY = (bestZone.start + bestZone.end) / 2;

            // However, if we're already reasonably close to the opponent and the current position is safe enough,
            // don't move unless we really need to
            var currentDistanceToOpponent = Math.Abs(playerY - opponentY);
            var isCurrentPositionReasonablySafe = !imminentThreats.Any(t => 
                Math.Abs(t.TargetY - playerY) < playerHeight * 0.6f && t.TimeLeft < 0.25f);

            // Increased the distance threshold and made safety check more restrictive
            if (currentDistanceToOpponent < 80 && isCurrentPositionReasonablySafe)
                return; // Stay put if we're close to opponent and relatively safe

            // Move towards the target, but with a larger deadzone to prevent jittering
            var movementThreshold = 40f; // Increased threshold for less jittery movement
            if (Math.Abs(targetY - playerY) > movementThreshold)
            {
                if (targetY > playerY)
                    _moveDown = true;
                else
                    _moveUp = true;
            }
        }

        private void HandlePlayerChasing()
        {
            // Don't override movement decisions from threat avoidance
            if (_moveUp || _moveDown)
                return;

            var opponent = GetOpponent();
            var currentDistanceToOpponent = Math.Abs(_player.Center.Y - opponent.Center.Y);
            
            // Create a larger deadzone where the CPU doesn't need to move
            // This prevents constant jittering and allows the CPU to stay still
            var chaseDeadzone = 50f; // Increased from 30 to 50 for more stillness
            
            // Only move if we're outside the acceptable range
            if (_player.Center.Y < opponent.Center.Y - chaseDeadzone)
                _moveDown = true;
            else if (_player.Center.Y > opponent.Center.Y + chaseDeadzone)
                _moveUp = true;
            // If within the deadzone, don't set any movement flags - stay still
        }

        private Player GetOpponent()
        {
            return _objectService.GetGameObjects<Player>().Single(x => x.Id != _player.Id);
        }

        private EffectModifiers GetModifiers()
        {
            var effectManager = _playerManagerFactory.GetEffectManager(_player.Id);
            return effectManager.GetModifiers();
        }

        private void HandlePhaseShiftDeploy(EffectModifiers modifiers)
        {
            var effectManager = _playerManagerFactory.GetEffectManager(_player);
            var phaseShifter = _objectService.GetGameObjects<PhaseShifter>()
                .SingleOrDefault(x => x.Owner == _player.Id);

            bool hasEffect = effectManager.HasEffect<PhaseShiftedEffect>();
            if (phaseShifter != null)
            {
                if(hasEffect)
                {
                    //
                }
                else 
                { 
                    if (_player.Center.Y > phaseShifter.Center.Y)
                        _moveUp = true;
                    else
                        _moveDown = true;
                }
                return;
            }

            if (phaseShifter == null && _deployWasReleased)
            {
                _deploy = true;
                _deployWasReleased = false;
                return;
            }
        }
    }
}