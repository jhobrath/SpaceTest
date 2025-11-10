using System.Numerics;

namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Aggressive combat positioning to align shots with opponent
    /// </summary>
    public class CombatPositioningStrategy : ICpuStrategy
    {
        public string Name => "Combat Positioning";
        public int Priority => 40;

        private const float OptimalDistance = 200f; // Back to more aggressive distance
        private const float AlignmentThreshold = 60f; 
        private const float TooCloseDistance = 120f;

        public bool ShouldExecute(CpuContext context)
        {
            // Activate when reasonably safe - not completely threat-free
            return !context.IsInDanger && !context.Threats.Any(t => t.DangerLevel > 0.4f);
        }

        public CpuDecision Execute(CpuContext context)
        {
            var toOpponent = context.OpponentPosition - context.CpuPosition;
            var distance = toOpponent.Length();
            
            var decision = new CpuDecision
            {
                Shoot = true,
                Reason = $"Combat positioning at distance {distance:F1}"
            };

            // More active vertical alignment
            var verticalOffset = toOpponent.Y;
            if (Math.Abs(verticalOffset) > AlignmentThreshold)
            {
                decision.MoveUp = verticalOffset < -AlignmentThreshold;
                decision.MoveDown = verticalOffset > AlignmentThreshold;
            }

            // Balanced horizontal positioning
            var horizontalOffset = toOpponent.X;
            
            if (distance < TooCloseDistance)
            {
                // Back away when too close
                decision.MoveRight = true;
                decision.Reason += " (backing away)";
            }
            else if (distance > OptimalDistance * 1.5f)
            {
                // Move closer when too far
                decision.MoveLeft = horizontalOffset < 0;
                decision.Reason += " (moving closer)";
            }
            else if (distance > OptimalDistance)
            {
                // Slight adjustment toward optimal range
                if (Math.Abs(verticalOffset) < AlignmentThreshold) // Only if aligned vertically
                {
                    decision.MoveLeft = true;
                    decision.Reason += " (optimizing distance)";
                }
            }

            return decision;
        }
    }
}