using System.Numerics;

namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Predictive dodging of incoming projectiles
    /// </summary>
    public class ProjectileDodgeStrategy : ICpuStrategy
    {
        public string Name => "Projectile Dodge";
        public int Priority => 80;

        private const float DodgeThreshold = 0.3f; // More selective - was 0.15f
        private const float PredictionTime = 2.0f;

        public bool ShouldExecute(CpuContext context)
        {
            return context.Threats.Any(t => t.DangerLevel > DodgeThreshold && t.CanDodge);
        }

        public CpuDecision Execute(CpuContext context)
        {
            var threats = context.Threats
                .Where(t => t.DangerLevel > DodgeThreshold && t.CanDodge)
                .OrderByDescending(t => t.DangerLevel)
                .ToList();

            var decision = new CpuDecision 
            { 
                Shoot = true, // Keep shooting while dodging
                Reason = $"Dodging {threats.Count} threats (max danger: {threats.FirstOrDefault()?.DangerLevel:F2})"
            };

            // Only stop shooting if in extreme danger
            if (threats.Any(t => t.DangerLevel > 0.8f))
            {
                decision.Shoot = false;
                decision.Reason += " (emergency focus)";
            }

            // Calculate dodge direction from top 3 threats only
            var dodgeDirection = Vector2.Zero;
            float totalWeight = 0f;

            foreach (var threat in threats.Take(3)) 
            {
                var weight = threat.DangerLevel;
                dodgeDirection += threat.SafeDirection * weight;
                totalWeight += weight;
            }

            if (totalWeight > 0)
            {
                dodgeDirection /= totalWeight;

                // Reasonable movement threshold
                const float MovementThreshold = 0.2f; 
                decision.MoveUp = dodgeDirection.Y < -MovementThreshold;
                decision.MoveDown = dodgeDirection.Y > MovementThreshold;
                decision.MoveLeft = dodgeDirection.X < -MovementThreshold;
                decision.MoveRight = dodgeDirection.X > MovementThreshold;

                // Only force movement for very high danger
                if (threats.Any(t => t.DangerLevel > 0.6f))
                {
                    if (Math.Abs(dodgeDirection.Y) > Math.Abs(dodgeDirection.X))
                    {
                        // Prioritize vertical movement for high danger
                        decision.MoveUp = dodgeDirection.Y < 0;
                        decision.MoveDown = dodgeDirection.Y > 0;
                        decision.MoveLeft = decision.MoveRight = false;
                    }
                }
            }

            return decision;
        }
    }
}