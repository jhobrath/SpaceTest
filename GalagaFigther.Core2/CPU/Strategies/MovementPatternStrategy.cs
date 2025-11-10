using System.Numerics;

namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Basic movement patterns and screen edge avoidance
    /// </summary>
    public class MovementPatternStrategy : ICpuStrategy
    {
        public string Name => "Movement Pattern";
        public int Priority => 20;

        private float _patternTime = 0f;
        private const float PatternCycleDuration = 4.0f;

        public bool ShouldExecute(CpuContext context)
        {
            return true; // Always applicable as fallback
        }

        public CpuDecision Execute(CpuContext context)
        {
            _patternTime += context.FrameTime;
            if (_patternTime > PatternCycleDuration) _patternTime = 0f;

            var decision = new CpuDecision
            {
                Shoot = true,
                Reason = "Default movement pattern"
            };

            // Avoid screen edges with high priority
            if (context.IsNearTopEdge)
            {
                decision.MoveDown = true;
                decision.Reason = "Avoiding top edge";
                return decision;
            }
            
            if (context.IsNearBottomEdge)
            {
                decision.MoveUp = true;
                decision.Reason = "Avoiding bottom edge";
                return decision;
            }

            if (context.IsNearRightEdge)
            {
                decision.MoveLeft = true;
                decision.Reason = "Avoiding right edge";
                return decision;
            }

            // Subtle movement pattern when no specific threat
            var normalizedTime = _patternTime / PatternCycleDuration;
            var waveOffset = (float)Math.Sin(normalizedTime * Math.PI * 2) * 0.5f;

            // Gentle vertical oscillation with bias towards center-top
            if (waveOffset > 0.2f && !context.IsNearTopEdge)
            {
                decision.MoveUp = true;
                decision.Reason = "Pattern movement up";
            }
            else if (waveOffset < -0.2f && !context.IsNearBottomEdge)
            {
                decision.MoveDown = true;
                decision.Reason = "Pattern movement down";
            }

            // Occasional horizontal adjustment to stay in optimal zone
            var horizontalDistance = Math.Abs(context.CpuPosition.X - context.OpponentPosition.X);
            if (horizontalDistance < 150f) // Too close
            {
                decision.MoveRight = true;
                decision.Reason = "Maintaining safe distance";
            }

            return decision;
        }
    }
}