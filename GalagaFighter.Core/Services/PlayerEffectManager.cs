using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerEffectManager
    {
        void AddEffect(Player player, PlayerEffect effect);
        void RemoveEffect(Player player, PlayerEffect effect);
        IReadOnlyList<PlayerEffect> GetStatusEffects(Player player);
        IReadOnlyList<PlayerEffect> GetProjectileEffects(Player player);
        PlayerEffect GetSelectedProjectileEffect(Player player);
        void SwitchProjectileEffect(Player player);
        void UpdateEffects(float frameTime);
    }

    public class PlayerEffectManager : IPlayerEffectManager
    {
        private readonly Dictionary<Player, List<PlayerEffect>> _statusEffects = new();
        private readonly Dictionary<Player, List<PlayerEffect>> _projectileEffects = new();
        private readonly Dictionary<Player, PlayerEffect> _selectedProjectileEffect = new();

        public void AddEffect(Player player, PlayerEffect effect)
        {
            if (!effect.IsProjectile)
            {
                if (!_statusEffects.ContainsKey(player))
                    _statusEffects[player] = new List<PlayerEffect>();
                _statusEffects[player].Add(effect);
                return;
            }

            if (!_projectileEffects.ContainsKey(player))
                _projectileEffects[player] = new List<PlayerEffect>();
            var effects = _projectileEffects[player];
            var duplicates = effects.Where(x => x.GetType() == effect.GetType()).ToList();
            foreach (var duplicate in duplicates)
                effects.Remove(duplicate);
            effects.Add(effect);
            _selectedProjectileEffect[player] = effect;
        }

        public void RemoveEffect(Player player, PlayerEffect effect)
        {
            if (!effect.IsProjectile)
            {
                if (_statusEffects.ContainsKey(player))
                    _statusEffects[player].Remove(effect);
                return;
            }

            if (_projectileEffects.ContainsKey(player))
            {
                var effects = _projectileEffects[player];
                effects.Remove(effect);
                if (_selectedProjectileEffect.ContainsKey(player) && _selectedProjectileEffect[player] == effect)
                {
                    _selectedProjectileEffect[player] = effects.FirstOrDefault();
                }
                // If no projectile effects remain, revert to the original default effect
                if (effects.Count == 0)
                {
                    // Try to find the original DefaultShootEffect (assumed to be first added)
                    var defaultEffect = effects.FirstOrDefault(e => e is DefaultShootEffect);
                    if (defaultEffect == null)
                    {
                        defaultEffect = new DefaultShootEffect();
                        effects.Add(defaultEffect);
                    }
                    _selectedProjectileEffect[player] = defaultEffect;
                }
            }
        }

        public IReadOnlyList<PlayerEffect> GetStatusEffects(Player player)
        {
            if (_statusEffects.ContainsKey(player))
                return _statusEffects[player];
            return Array.Empty<PlayerEffect>();
        }

        public IReadOnlyList<PlayerEffect> GetProjectileEffects(Player player)
        {
            if (_projectileEffects.ContainsKey(player))
                return _projectileEffects[player];
            return Array.Empty<PlayerEffect>();
        }

        public PlayerEffect GetSelectedProjectileEffect(Player player)
        {
            if (_selectedProjectileEffect.ContainsKey(player))
                return _selectedProjectileEffect[player];
            return null;
        }

        public void SwitchProjectileEffect(Player player)
        {
            if (!_projectileEffects.ContainsKey(player)) return;
            var effects = _projectileEffects[player];
            if (effects.Count == 0) return;
            var selected = GetSelectedProjectileEffect(player);
            var idx = effects.IndexOf(selected);
            if (effects.Count > idx + 1)
                _selectedProjectileEffect[player] = effects[idx + 1];
            else
                _selectedProjectileEffect[player] = effects[0];
        }

        public void UpdateEffects(float frameTime)
        {
            foreach (var kvp in _statusEffects)
            {
                var player = kvp.Key;
                var effects = kvp.Value;
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    effects[i].OnUpdate(frameTime);
                    if (!effects[i].IsActive)
                        RemoveEffect(player, effects[i]);
                }
            }
            foreach (var kvp in _projectileEffects)
            {
                var player = kvp.Key;
                var effects = kvp.Value;
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    effects[i].OnUpdate(frameTime);
                    if (!effects[i].IsActive)
                        RemoveEffect(player, effects[i]);
                }
            }
        }
    }
}
