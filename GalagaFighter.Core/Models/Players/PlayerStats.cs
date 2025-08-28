namespace GalagaFighter.Core.Models.Players
{
    public class PlayerStats
    {
        public float Damage { get; set; } = 1f;
        public float FireRateMultiplier { get; set; } = 1f;
        public float SpeedMultiplier { get; set; } = 1f;
        public float Shield { get; set; } = 1f;
        public float Health { get; set; } = 1f; // Multiplier for health, default 1
        public bool DoubleShot { get; set; } = false;
    }
}
