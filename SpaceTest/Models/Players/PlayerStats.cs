using System.Collections.Generic;
using System.Linq;
using GalagaFighter.Models.Effects;

namespace GalagaFighter.Models.Players
{
    public class PlayerStats
    {
        public int Health { get; private set; }
        public int MaxBullets { get; private set; }
        public float FireRateMultiplier { get; private set; }
        private const int baseBulletCapacity = 8;

        private Player _player;

        private readonly List<PlayerEffect> activeEffects = new List<PlayerEffect>();

        public PlayerStats()
        {
            Health = 100;
            MaxBullets = baseBulletCapacity;
            FireRateMultiplier = 1;
        }

        public PlayerStats(Player player)
        {
            Health = 100;
            MaxBullets = baseBulletCapacity;
            FireRateMultiplier = 1;
            // Always add default shooting effect
            activeEffects.Add(new DefaultShootEffect(player));

            _player = player;
        }

        public void UpdateEffects(float frameTime)
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.OnUpdate(frameTime);
                if (effect.ShouldDeactivate())
                {
                    effect.OnDeactivate();
                    activeEffects.RemoveAt(i);
                }
            }

            if(activeEffects.All(x => x is not ProjectileEffect))
            {
                // Ensure there's always a default shooting effect
                activeEffects.Add(new DefaultShootEffect(_player));
            }
        }

        public void AddEffect(Player player, PlayerEffect effect)
        {
            if (effect is ProjectileEffect)
            {
                activeEffects.ForEach(x => x.OnDeactivate());
                activeEffects.RemoveAll(x => x is ProjectileEffect);
            }

            if(!effect.AllowSelfStacking)
            {
                var effectType = effect.GetType().Name;
                for(var i = activeEffects.Count-1;i>=0;i--)
                {
                    if (activeEffects[i].GetType().Name == effectType)
                    {
                        activeEffects[i].OnDeactivate();
                        activeEffects.RemoveAt(i);
                    }
                }
            }

            activeEffects.Add(effect);
            effect.OnActivate();
        }

        public int GetActiveEffectCount<T>() where T : PlayerEffect
        {
            return activeEffects.Count(e => e is T);
        }

        public T GetFirstEffect<T>() where T : PlayerEffect
        {
            return activeEffects.OfType<T>().FirstOrDefault();
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        // Methods for PowerUpEffects to use
        public void ModifyFireRate(float multiplier)
        {
            FireRateMultiplier *= multiplier;
        }

        public void AddBullets(int amount)
        {
            MaxBullets += amount;
        }

        // For effects to query if player is currently affected
        public bool HasEffect<T>() where T : PlayerEffect
        {
            return activeEffects.Any(e => e is T);
        }

        public IEnumerable<PlayerEffect> GetActiveEffects() => activeEffects;
    }
}