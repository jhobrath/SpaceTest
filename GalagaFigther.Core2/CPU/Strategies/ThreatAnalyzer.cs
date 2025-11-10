using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Services;
using System.Numerics;

namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Analyzes incoming projectiles and calculates threat levels
    /// </summary>
    public class ThreatAnalyzer
    {
        private const float MaxThreatDistance = 400f; // Back to original
        private const float CollisionRadius = 50f;   // Slightly increased from 30f

        public List<ProjectileThreat> AnalyzeThreats(CpuContext context)
        {
            var threats = new List<ProjectileThreat>();

            foreach (var projectile in context.IncomingProjectiles)
            {
                var threat = AnalyzeProjectile(projectile, context);
                if (threat.DangerLevel > 0.1f) // Back to reasonable threshold
                {
                    threats.Add(threat);
                }
            }

            return threats.OrderByDescending(t => t.DangerLevel).ToList();
        }

        private ProjectileThreat AnalyzeProjectile(Projectile projectile, CpuContext context)
        {
            var projectilePos = projectile.WorldPosition;
            var projectileVel = projectile.Speed;
            var cpuPos = context.CpuPosition;

            // Calculate if projectile is heading towards CPU
            var toTarget = cpuPos - projectilePos;
            var distance = toTarget.Length();
            
            if (distance > MaxThreatDistance)
            {
                return new ProjectileThreat
                {
                    Projectile = projectile,
                    DangerLevel = 0f,
                    CanDodge = false
                };
            }

            // Predict intersection point
            var timeToImpact = PredictCollisionTime(projectilePos, projectileVel, cpuPos, Vector2.Zero);
            var predictedImpactPoint = projectilePos + projectileVel * timeToImpact;
            
            // Calculate danger level
            var impactDistance = Vector2.Distance(predictedImpactPoint, cpuPos);
            var dangerLevel = CalculateDangerLevel(distance, impactDistance, timeToImpact, projectileVel.Length());

            // Calculate safe direction without excessive bias
            var safeDirection = CalculateSafeDirection(projectilePos, projectileVel, cpuPos, context);

            return new ProjectileThreat
            {
                Projectile = projectile,
                DangerLevel = dangerLevel,
                TimeToImpact = timeToImpact,
                PredictedImpactPoint = predictedImpactPoint,
                SafeDirection = safeDirection,
                CanDodge = timeToImpact > 0.3f && dangerLevel < 0.9f
            };
        }

        private float PredictCollisionTime(Vector2 projectilePos, Vector2 projectileVel, Vector2 targetPos, Vector2 targetVel)
        {
            var relativeVel = projectileVel - targetVel;
            var relativePos = targetPos - projectilePos;

            if (relativeVel.LengthSquared() < 0.01f)
                return float.MaxValue;

            var timeToClosest = Vector2.Dot(relativePos, relativeVel) / relativeVel.LengthSquared();
            return Math.Clamp(timeToClosest, 0f, 5f); // Reasonable clamp
        }

        private float CalculateDangerLevel(float distance, float impactDistance, float timeToImpact, float projectileSpeed)
        {
            // Proximity factor
            var proximityFactor = Math.Max(0f, 1f - (distance / MaxThreatDistance));
            
            // Impact accuracy factor
            var accuracyFactor = Math.Max(0f, 1f - (impactDistance / CollisionRadius));
            
            // Time urgency factor
            var urgencyFactor = timeToImpact < 1.5f ? Math.Max(0f, 1f - (timeToImpact / 1.5f)) : 0f;
            
            // Speed factor
            var speedFactor = Math.Min(1f, projectileSpeed / 400f);

            // Balanced combination
            var dangerLevel = proximityFactor * 0.25f + 
                             accuracyFactor * 0.4f + 
                             urgencyFactor * 0.25f + 
                             speedFactor * 0.1f;
            
            return Math.Clamp(dangerLevel, 0f, 1f);
        }

        private Vector2 CalculateSafeDirection(Vector2 projectilePos, Vector2 projectileVel, Vector2 cpuPos, CpuContext context)
        {
            // Get perpendicular direction to projectile movement
            var perpendicular = Vector2.Zero;
            if (projectileVel.LengthSquared() > 0.01f)
            {
                perpendicular = new Vector2(-projectileVel.Y, projectileVel.X);
                perpendicular = Vector2.Normalize(perpendicular);
            }
            else
            {
                // If projectile isn't moving, just move away from it
                var awayFromProjectile = cpuPos - projectilePos;
                if (awayFromProjectile.LengthSquared() > 0.01f)
                    perpendicular = Vector2.Normalize(awayFromProjectile);
            }

            // Choose direction away from projectile
            var toProjectile = projectilePos - cpuPos;
            if (toProjectile.LengthSquared() > 0.01f && Vector2.Dot(perpendicular, toProjectile) > 0)
                perpendicular = -perpendicular;

            // Moderate screen edge avoidance - don't hug walls
            if (context.IsNearTopEdge && perpendicular.Y < -0.3f)
                perpendicular.Y = Math.Max(-0.3f, perpendicular.Y);
            
            if (context.IsNearBottomEdge && perpendicular.Y > 0.3f)
                perpendicular.Y = Math.Min(0.3f, perpendicular.Y);
            
            if (context.IsNearRightEdge && perpendicular.X > 0.3f)
                perpendicular.X = Math.Min(-0.3f, perpendicular.X);

            // Minimal bias - just slight preference for staying in middle-upper area
            perpendicular.Y -= 0.1f; // Very slight upward bias
            
            // Ensure we have a valid direction
            if (perpendicular.LengthSquared() < 0.01f)
                perpendicular = new Vector2(0f, -1f); // Default to up

            return Vector2.Normalize(perpendicular);
        }
    }
}