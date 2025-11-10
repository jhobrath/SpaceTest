using GalagaFighter.Core2.GameObjects.PowerUps;
using System.Numerics;

namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Opportunistic power-up collection
    /// </summary>
    public class PowerUpCollectionStrategy : ICpuStrategy
    {
        public string Name => "Power-Up Collection";
        public int Priority => 60;

        private const float MaxPowerUpDistance = 300f;
        private const float CollectionRadius = 50f;

        public bool ShouldExecute(CpuContext context)
        {
            return context.AvailablePowerUps.Any() && 
                   !context.IsInDanger && // Only collect when safe
                   context.AvailablePowerUps.Any(p => Vector2.Distance(p.WorldPosition, context.CpuPosition) < MaxPowerUpDistance);
        }

        public CpuDecision Execute(CpuContext context)
        {
            var nearbyPowerUps = context.AvailablePowerUps
                .Where(p => Vector2.Distance(p.WorldPosition, context.CpuPosition) < MaxPowerUpDistance)
                .OrderBy(p => Vector2.Distance(p.WorldPosition, context.CpuPosition))
                .ToList();

            if (!nearbyPowerUps.Any())
                return CpuDecision.None;

            var targetPowerUp = nearbyPowerUps.First();
            var direction = targetPowerUp.WorldPosition - context.CpuPosition;
            var distance = direction.Length();

            var decision = new CpuDecision
            {
                Shoot = true, // Keep shooting
                Reason = $"Collecting power-up at distance {distance:F1}"
            };

            if (distance > CollectionRadius)
            {
                direction = Vector2.Normalize(direction);
                
                const float MovementThreshold = 0.3f;
                decision.MoveUp = direction.Y < -MovementThreshold;
                decision.MoveDown = direction.Y > MovementThreshold;
                decision.MoveLeft = direction.X < -MovementThreshold;
                decision.MoveRight = direction.X > MovementThreshold;
            }

            return decision;
        }
    }
}