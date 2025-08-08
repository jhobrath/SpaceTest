using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Models.Players
{
    public class PlayerStats
    {
        public int Health { get; private set; }
        public int MaxBullets { get; private set; }
        private const int baseBulletCapacity = 8;
        private const int bulletsPerPowerUp = 2;
        
        private List<float> iceEffectTimers = new List<float>();
        private const float iceEffectDuration = 5.0f;
        private const float maxSlowFactor = 0.1f;

        public float IceShotTimer { get; private set; }
        public float SlowTimer { get; private set; }
        public bool HasWall { get; private set; }

        public PlayerStats()
        {
            Health = 100;
            MaxBullets = baseBulletCapacity;
            IceShotTimer = 0;
            SlowTimer = 0;
            HasWall = false;
        }

        public void UpdateEffects(float frameTime)
        {
            if (IceShotTimer > 0) IceShotTimer -= frameTime;

            // Update ice effect timers
            for (int i = iceEffectTimers.Count - 1; i >= 0; i--)
            {
                iceEffectTimers[i] -= frameTime;
                if (iceEffectTimers[i] <= 0)
                {
                    iceEffectTimers.RemoveAt(i);
                }
            }

            // Update SlowTimer for backward compatibility
            SlowTimer = iceEffectTimers.Count > 0 ? iceEffectTimers.Max() : 0;
        }

        public float CalculateSlowIntensity()
        {
            if (iceEffectTimers.Count == 0) return 1.0f;

            float slowPerEffect = 0.3f;
            float totalSlow = iceEffectTimers.Count * slowPerEffect;
            float clampedSlow = Math.Min(totalSlow, 1.0f - maxSlowFactor);

            return 1.0f - clampedSlow;
        }

        public void ApplySlowEffect(float duration)
        {
            iceEffectTimers.Add(duration);
            
            if (iceEffectTimers.Count > 3)
            {
                iceEffectTimers.Remove(iceEffectTimers.Min());
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void ApplyPowerUpEffect(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.BulletCapacity:
                    MaxBullets += bulletsPerPowerUp;
                    break;
                case PowerUpType.IceShot:
                    IceShotTimer = 10.0f;
                    break;
                case PowerUpType.Wall:
                    HasWall = true;
                    break;
            }
        }

        public void ConsumeWall()
        {
            HasWall = false;
        }

        public void ResetIceShotTimer()
        {
            IceShotTimer = 0;
        }

        public int IceEffectCount => iceEffectTimers.Count;
        public float CurrentSlowIntensity => 1.0f - CalculateSlowIntensity();
    }
}