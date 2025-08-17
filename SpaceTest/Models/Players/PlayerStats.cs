using System.Collections.Generic;
using System.Linq;
using GalagaFighter.Models.Effects;

namespace GalagaFighter.Models.Players
{
    public class PlayerStats
    {
        public int Health { get; set; }
        public float FireRateMultiplier => _effects.Select(x => x.FireRateMultiplier).Aggregate((a,b) => a*b);

        private readonly List<PlayerEffect> _effects = [];
        private Player _player;


        public PlayerStats()
        {
            Health = 100;
        }

        public PlayerStats(Player player)
        {
            Health = 100;
            _effects.Add(new DefaultShootEffect(player));
            _player = player;
        }

        public void UpdateEffects(float frameTime)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];
                effect.OnUpdate(frameTime);
                if (effect.ShouldDeactivate())
                {
                    effect.OnDeactivate();
                    _effects.RemoveAt(i);

                    if (_effects.All(x => x is not ProjectileEffect))
                        _effects.Add(new DefaultShootEffect(_player));
                }
            }
        }

        public void AddEffect(Player player, PlayerEffect effect)
        {
            if (effect is ProjectileEffect)
            {
                _effects.ForEach(x => x.OnDeactivate());
                _effects.RemoveAll(x => x is ProjectileEffect);
            }

            if(!effect.AllowSelfStacking)
            {
                var effectType = effect.GetType().Name;
                for(var i = _effects.Count-1;i>=0;i--)
                {
                    if (_effects[i].GetType().Name == effectType)
                    {
                        _effects[i].OnDeactivate();
                        _effects.RemoveAt(i);
                    }
                }
            }

            _effects.Add(effect);
            effect.OnActivate();
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }


        // For effects to query if player is currently affected
        public bool HasEffect<T>() where T : PlayerEffect
        {
            return _effects.Any(e => e is T);
        }

        public IEnumerable<PlayerEffect> GetActiveEffects() => _effects;
    }
}