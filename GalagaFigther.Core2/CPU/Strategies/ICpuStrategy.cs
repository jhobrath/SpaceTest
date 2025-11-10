using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.GameObjects.PowerUps;
using System.Numerics;
using Raylib_cs;

namespace GalagaFighter.Core2.CPU.Strategies
{
    public interface ICpuStrategy
    {
        string Name { get; }
        int Priority { get; }
        bool ShouldExecute(CpuContext context);
        CpuDecision Execute(CpuContext context);
    }

    public class CpuContext
    {
        public Player CpuPlayer { get; set; }
        public Player OpponentPlayer { get; set; }
        public Vector2 CpuPosition { get; set; }
        public Vector2 OpponentPosition { get; set; }
        public List<Projectile> IncomingProjectiles { get; set; } = new();
        public List<PowerUp> AvailablePowerUps { get; set; } = new();
        public float FrameTime { get; set; }
        public float GameTime { get; set; }
        public Rectangle ScreenBounds { get; set; }
        
        // Threat analysis
        public List<ProjectileThreat> Threats { get; set; } = new();
        public bool IsInDanger => Threats.Any(t => t.DangerLevel > 0.5f);
        public ProjectileThreat? MostDangerousThreat => Threats.OrderByDescending(t => t.DangerLevel).FirstOrDefault();
        
        // Screen positioning
        public bool IsNearRightEdge => CpuPosition.X > ScreenBounds.Width * 0.8f;
        public bool IsNearLeftEdge => CpuPosition.X < ScreenBounds.Width * 0.2f;
        public bool IsNearTopEdge => CpuPosition.Y < ScreenBounds.Height * 0.1f;
        public bool IsNearBottomEdge => CpuPosition.Y > ScreenBounds.Height * 0.9f;
    }

    public class CpuDecision
    {
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool Shoot { get; set; }
        public bool Shield { get; set; }
        public bool Deploy { get; set; }
        public bool Defend { get; set; }
        public string Reason { get; set; } = "";

        public static CpuDecision None => new CpuDecision { Reason = "No action needed" };
        public static CpuDecision AlwaysShoot => new CpuDecision { Shoot = true, Reason = "Always shooting" };
    }

    public class ProjectileThreat
    {
        public Projectile Projectile { get; set; }
        public float DangerLevel { get; set; } // 0.0 to 1.0
        public float TimeToImpact { get; set; }
        public Vector2 PredictedImpactPoint { get; set; }
        public Vector2 SafeDirection { get; set; }
        public bool CanDodge { get; set; }
    }
}