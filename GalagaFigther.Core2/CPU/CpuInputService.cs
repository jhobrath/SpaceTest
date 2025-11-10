using GalagaFighter.Core2.CPU;
using GalagaFighter.Core2.CPU.Strategies;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.CPU
{
    public interface ICpuInputService
    {
        void SetPlayer(Player player);
        void Update(float frameTime);
        void SetConfiguration(CpuConfiguration configuration);
        CpuConfiguration GetConfiguration();
        string GetDebugInfo();
    }

    public class CpuInputService : IInputMappings, ICpuInputService
    {
        private bool _up = false;
        private bool _shoot = true;
        private bool _shield = false;
        private bool _right = false;
        private bool _left = false;
        private bool _down = false;
        private bool _deploy = false;
        private bool _defend = false;
        
        private Player? _player;
        private Player? _opponent;

        // Decision making system
        private readonly ICpuDecisionEngine _decisionEngine;
        private CpuDecision _lastDecision = CpuDecision.AlwaysShoot;
        private CpuConfiguration _configuration;

        // Input smoothing and timing
        private float _decisionCooldown = 0f;
        
        // Performance tracking
        private float _totalGameTime = 0f;
        private int _decisionsCount = 0;
        private string _lastDecisionReason = "";

        public bool IsDefendDown() => _defend;
        public bool IsDeployTurretDown() => _deploy;
        public bool IsDownDown() => _down;
        public bool IsLeftDown() => _left;
        public bool IsRightDown() => _right;
        public bool IsShieldDown() => _shield;
        public bool IsShootDown() => _shoot;
        public bool IsUpDown() => _up;

        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public CpuInputService(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
            _decisionEngine = new CpuDecisionEngine(objectService, gameDataRegistry);
            
            // Use a more defensive default configuration
            _configuration = new CpuConfiguration
            {
                AggressionLevel = 0.5f,      // Less aggressive
                ReactionTime = 0.9f,         // Fast reactions  
                AccuracyLevel = 0.7f,        // Good accuracy
                RiskTaking = 0.3f,           // Very conservative
                DecisionUpdateRate = 0.08f,  // Fast decision updates
                ThreatAvoidanceWeight = 1.2f, // Prioritize threat avoidance
                PowerUpPriorityWeight = 0.4f, // Lower power-up priority
                CombatAggressivenessWeight = 0.5f, // Less combat aggression
                DangerThreshold = 0.15f,     // Very sensitive to danger
                EmergencyThreshold = 0.5f,   // Earlier emergency response
                PowerUpMaxDistance = 250f    // Shorter power-up collection range
            };
        }

        public void SetConfiguration(CpuConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public CpuConfiguration GetConfiguration() => _configuration;

        public void SetPlayer(Player player)
        {
            _player = player;
            _opponent = _objectService.GetOpponent(_player);
            _decisionEngine.SetPlayer(player);
        }

        public void Update(float frameTime)
        {
            if (_player == null) return;

            _totalGameTime += frameTime;
            _decisionCooldown -= frameTime;

            // Update decisions at controlled rate based on configuration
            if (_decisionCooldown <= 0f)
            {
                _decisionCooldown = _configuration.DecisionUpdateRate;
                
                // Add some randomness based on reaction time (lower reaction time = more consistent)
                var reactionVariance = (1f - _configuration.ReactionTime) * 0.05f;
                _decisionCooldown += (float)(Random.Shared.NextDouble() - 0.5) * reactionVariance;

                _lastDecision = _decisionEngine.MakeDecision(frameTime);
                _lastDecisionReason = _lastDecision.Reason;
                _decisionsCount++;

                // Apply configuration modifiers to the decision
                ApplyConfigurationModifiers(_lastDecision);
            }

            // Apply the current decision to input states
            ApplyDecision(_lastDecision);
        }

        private void ApplyConfigurationModifiers(CpuDecision decision)
        {
            // Reduce shooting frequency based on accuracy level
            if (decision.Shoot && Random.Shared.NextDouble() > _configuration.AccuracyLevel)
            {
                decision.Shoot = false;
                decision.Reason += " (accuracy miss)";
            }

            // Add some hesitation in movement based on reaction time
            if (_configuration.ReactionTime < 0.8f)
            {
                var hesitationChance = (0.8f - _configuration.ReactionTime) * 0.3f;
                if (Random.Shared.NextDouble() < hesitationChance)
                {
                    decision.MoveUp = decision.MoveDown = decision.MoveLeft = decision.MoveRight = false;
                    decision.Reason += " (hesitation)";
                }
            }

            // Risk-taking modifications
            if (_configuration.RiskTaking > 0.7f)
            {
                // High risk-taking: more aggressive positioning
                if (!decision.MoveLeft && Random.Shared.NextDouble() < 0.1f)
                {
                    decision.MoveLeft = true;
                    decision.Reason += " (aggressive)";
                }
            }
        }

        private void ApplyDecision(CpuDecision decision)
        {
            _up = decision.MoveUp;
            _down = decision.MoveDown;
            _left = decision.MoveLeft;
            _right = decision.MoveRight;
            _shoot = decision.Shoot;
            _shield = decision.Shield;
            _deploy = decision.Deploy;
            _defend = decision.Defend;
        }

        /// <summary>
        /// Get opponent projectiles for external analysis if needed
        /// </summary>
        public List<Projectile> GetOpponentProjectiles()
        {
            if (_opponent == null) return new List<Projectile>();
            
            var projectiles = _objectService.GetChildren<Projectile>(_opponent);
            return projectiles.ToList();
        }

        /// <summary>
        /// Get debug information about current CPU decision and performance
        /// </summary>
        public string GetDebugInfo()
        {
            var avgDecisionsPerSecond = _totalGameTime > 0 ? _decisionsCount / _totalGameTime : 0;
            
            return $"CPU: {_lastDecisionReason}\n" +
                   $"Config: {_configuration.AggressionLevel:F1}A/{_configuration.ReactionTime:F1}R/{_configuration.AccuracyLevel:F1}Acc\n" +
                   $"Perf: {avgDecisionsPerSecond:F1} DPS, {_decisionsCount} total decisions";
        }

        /// <summary>
        /// Reset performance tracking (useful when restarting games)
        /// </summary>
        public void ResetTracking()
        {
            _totalGameTime = 0f;
            _decisionsCount = 0;
            _lastDecisionReason = "";
        }
    }
}
