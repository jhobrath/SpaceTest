using GalagaFighter.Core2.CPU.Strategies;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using System.Numerics;
using Raylib_cs;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.GameObjects.PowerUps;

namespace GalagaFighter.Core2.CPU
{
    public interface ICpuDecisionEngine
    {
        void SetPlayer(Player player);
        CpuDecision MakeDecision(float frameTime);
    }

    public class CpuDecisionEngine : ICpuDecisionEngine
    {
        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly ThreatAnalyzer _threatAnalyzer;
        private readonly List<ICpuStrategy> _strategies;

        private Player? _cpuPlayer;
        private Player? _opponentPlayer;
        private float _gameTime = 0f;

        public CpuDecisionEngine(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
            _threatAnalyzer = new ThreatAnalyzer();

            // Initialize strategies in priority order
            _strategies = new List<ICpuStrategy>
            {
                new EmergencyEvasionStrategy(),
                new ProjectileDodgeStrategy(),
                new PowerUpCollectionStrategy(),
                new CombatPositioningStrategy(),
                new MovementPatternStrategy()
            };
        }

        public void SetPlayer(Player player)
        {
            _cpuPlayer = player;
            _opponentPlayer = _objectService.GetOpponent(player);
        }

        public CpuDecision MakeDecision(float frameTime)
        {
            if (_cpuPlayer == null || _opponentPlayer == null)
                return CpuDecision.AlwaysShoot;

            _gameTime += frameTime;

            // Build context for decision making
            var context = BuildContext(frameTime);

            // Analyze threats
            context.Threats = _threatAnalyzer.AnalyzeThreats(context);

            // Execute strategies in priority order
            foreach (var strategy in _strategies.OrderByDescending(s => s.Priority))
            {
                if (strategy.ShouldExecute(context))
                {
                    var decision = strategy.Execute(context);
                    decision.Reason = $"[{strategy.Name}] {decision.Reason}";
                    return decision;
                }
            }

            // Fallback
            return CpuDecision.AlwaysShoot;
        }

        private CpuContext BuildContext(float frameTime)
        {
            var gameState = _gameDataRegistry.Get<GameState>();
            var incomingProjectiles = GetIncomingProjectiles();
            var availablePowerUps = GetAvailablePowerUps();

            return new CpuContext
            {
                CpuPlayer = _cpuPlayer!,
                OpponentPlayer = _opponentPlayer!,
                CpuPosition = _cpuPlayer!.WorldPosition,
                OpponentPosition = _opponentPlayer!.WorldPosition,
                IncomingProjectiles = incomingProjectiles,
                AvailablePowerUps = availablePowerUps,
                FrameTime = frameTime,
                GameTime = _gameTime,
                ScreenBounds = new Rectangle(0, 0, (int)gameState.ScreenSize.X, (int)gameState.ScreenSize.Y)
            };
        }

        private List<Projectile> GetIncomingProjectiles()
        {
            if (_opponentPlayer == null) return new List<Projectile>();

            // Get ALL opponent projectiles, then filter more intelligently
            return _objectService.GetChildren<Projectile>(_opponentPlayer)
                .Where(p => IsProjectilePotentialThreat(p, _cpuPlayer!.WorldPosition))
                .ToList();
        }

        private List<PowerUp> GetAvailablePowerUps()
        {
            return _objectService.GetAll<PowerUp>()
                .Where(p => p.IsActive)
                .ToList();
        }

        private bool IsProjectilePotentialThreat(Projectile projectile, Vector2 targetPosition)
        {
            var toTarget = targetPosition - projectile.WorldPosition;
            var distance = toTarget.Length();
            
            // Don't bother with projectiles that are very far away
            if (distance > 500f) return false;
            
            var projectileSpeed = projectile.Speed;
            
            // If projectile has no speed, it's not a threat
            if (projectileSpeed.LengthSquared() < 0.1f) return false;
            
            var projectileDirection = Vector2.Normalize(projectileSpeed);
            var toTargetDirection = Vector2.Normalize(toTarget);
            
            // Much more lenient threshold - anything moving roughly toward the CPU
            var dotProduct = Vector2.Dot(toTargetDirection, projectileDirection);
            
            // Consider it a potential threat if:
            // 1. It's moving toward us (dot > 0)
            // 2. OR it's close enough that direction doesn't matter much (< 100 pixels)
            return dotProduct > 0f || distance < 100f;
        }
    }
}