namespace GalagaFighter.Core2.CPU.Strategies
{
    /// <summary>
    /// Configuration settings for CPU behavior and difficulty
    /// </summary>
    public class CpuConfiguration
    {
        // Difficulty settings (0.0 to 1.0)
        public float AggressionLevel { get; set; } = 0.7f;
        public float ReactionTime { get; set; } = 0.8f;
        public float AccuracyLevel { get; set; } = 0.6f;
        public float RiskTaking { get; set; } = 0.5f;

        // Behavior weights
        public float ThreatAvoidanceWeight { get; set; } = 1.0f;
        public float PowerUpPriorityWeight { get; set; } = 0.6f;
        public float CombatAggressivenessWeight { get; set; } = 0.8f;
        
        // Timing parameters
        public float DecisionUpdateRate { get; set; } = 0.1f; // seconds between decision updates
        public float PatternChangeRate { get; set; } = 4.0f;  // seconds between movement pattern changes

        // Thresholds
        public float DangerThreshold { get; set; } = 0.4f;
        public float EmergencyThreshold { get; set; } = 0.8f;
        public float PowerUpMaxDistance { get; set; } = 300f;

        /// <summary>
        /// Creates a configuration for Easy difficulty
        /// </summary>
        public static CpuConfiguration Easy => new()
        {
            AggressionLevel = 0.4f,
            ReactionTime = 0.5f,
            AccuracyLevel = 0.3f,
            RiskTaking = 0.2f,
            DecisionUpdateRate = 0.15f,
            PowerUpPriorityWeight = 0.3f
        };

        /// <summary>
        /// Creates a configuration for Normal difficulty  
        /// </summary>
        public static CpuConfiguration Normal => new()
        {
            AggressionLevel = 0.7f,
            ReactionTime = 0.8f,
            AccuracyLevel = 0.6f,
            RiskTaking = 0.5f,
            DecisionUpdateRate = 0.1f,
            PowerUpPriorityWeight = 0.6f
        };

        /// <summary>
        /// Creates a configuration for Hard difficulty
        /// </summary>
        public static CpuConfiguration Hard => new()
        {
            AggressionLevel = 0.9f,
            ReactionTime = 0.95f,
            AccuracyLevel = 0.8f,
            RiskTaking = 0.7f,
            DecisionUpdateRate = 0.05f,
            PowerUpPriorityWeight = 0.8f,
            DangerThreshold = 0.6f
        };

        /// <summary>
        /// Creates a configuration for Expert difficulty - very challenging
        /// </summary>
        public static CpuConfiguration Expert => new()
        {
            AggressionLevel = 1.0f,
            ReactionTime = 1.0f,
            AccuracyLevel = 0.95f,
            RiskTaking = 0.8f,
            DecisionUpdateRate = 0.03f,
            PowerUpPriorityWeight = 0.9f,
            DangerThreshold = 0.7f,
            EmergencyThreshold = 0.9f
        };
    }
}