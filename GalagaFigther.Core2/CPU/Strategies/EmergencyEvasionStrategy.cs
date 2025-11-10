using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using System.Numerics;

namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Emergency evasive maneuvers - highest priority
    /// </summary>
    public class EmergencyEvasionStrategy : ICpuStrategy
    {
        public string Name => "Emergency Evasion";
        public int Priority => 100;

        public bool ShouldExecute(CpuContext context)
        {
            // Only trigger for truly dangerous situations
            return context.IsInDanger && context.MostDangerousThreat?.DangerLevel > 0.7f;
        }

        public CpuDecision Execute(CpuContext context)
        {
            var threat = context.MostDangerousThreat!;
            var decision = new CpuDecision { Reason = $"Emergency evasion from {threat.DangerLevel:F2} danger" };

            // Calculate evasive direction
            var safeDirection = CalculateSafeDirection(context, threat);
            
            // More decisive movement for true emergencies
            decision.MoveUp = safeDirection.Y < -0.2f;
            decision.MoveDown = safeDirection.Y > 0.2f;
            decision.MoveLeft = safeDirection.X < -0.2f;
            decision.MoveRight = safeDirection.X > 0.2f;
            
            // Stop shooting only in extreme danger
            decision.Shoot = threat.DangerLevel < 0.9f;
            
            return decision;
        }

        private Vector2 CalculateSafeDirection(CpuContext context, ProjectileThreat threat)
        {
            var projectilePos = threat.Projectile.WorldPosition;
            var cpuPos = context.CpuPosition;
            var projectileVel = threat.Projectile.Speed;

            // Calculate perpendicular direction to projectile movement
            var perpendicular = new Vector2(-projectileVel.Y, projectileVel.X);
            if (perpendicular.Length() > 0)
                perpendicular = Vector2.Normalize(perpendicular);

            // Choose direction away from projectile
            if (Vector2.Distance(projectilePos, cpuPos) > 0.1f)
            {
                var toProjectile = Vector2.Normalize(projectilePos - cpuPos);
                if (Vector2.Dot(perpendicular, toProjectile) > 0)
                    perpendicular = -perpendicular;
            }

            // Moderate upward bias for safety
            perpendicular.Y -= 0.2f;

            // Reasonable screen edge avoidance
            if (context.IsNearTopEdge) perpendicular.Y = Math.Max(0.3f, perpendicular.Y);
            if (context.IsNearBottomEdge) perpendicular.Y = Math.Min(-0.3f, perpendicular.Y);
            if (context.IsNearRightEdge) perpendicular.X = Math.Min(-0.3f, perpendicular.X);

            // Ensure valid direction
            if (perpendicular.LengthSquared() < 0.01f)
                perpendicular = new Vector2(0f, -1f);

            return Vector2.Normalize(perpendicular);
        }
    }
}