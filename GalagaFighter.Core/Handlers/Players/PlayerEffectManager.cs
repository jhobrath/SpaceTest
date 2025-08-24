using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerEffectManager
    {
        void AddEffect(PlayerEffect effect);
        void RemoveEffect(PlayerEffect effect);
        void SwitchEffect();
        EffectModifiers GetModifiers();
    }

    public interface IExposedPlayerEffectManager : IPlayerEffectManager
    {
        List<PlayerEffect> Effects { get; }
        PlayerEffect SelectedProjectile { get; }
    }

    public class PlayerEffectManager : IPlayerEffectManager, IExposedPlayerEffectManager
    {
        List<PlayerEffect> IExposedPlayerEffectManager.Effects => _effects; 
        PlayerEffect IExposedPlayerEffectManager.SelectedProjectile => _selectedProjectile; 

        private EffectModifiers _modifiers = new EffectModifiers();
        private PlayerEffect _selectedProjectile = new DefaultShootEffect();
        private readonly List<PlayerEffect> _effects = [];

        public PlayerEffectManager()
        {
            AddEffect(_selectedProjectile);
        }

        public void AddEffect(PlayerEffect newEffect)
        {
            _effects.Add(newEffect);

            LimitEffectCount(newEffect);
            UpdateModifiers();
        }

        private void LimitEffectCount<T>(T newEffect)
            where T : PlayerEffect
        {
            if (newEffect.MaxCount == 0)
                return;

            var ofType = _effects.Count(x => x.GetType() == typeof(T));
            if (ofType < newEffect.MaxCount)
                return;

            var first = _effects.First(x => x.GetType() == typeof(T));
            _effects.Remove(first);
        }

        public void RemoveEffect(PlayerEffect effect)
        {
            _effects.RemoveAll(e => e == effect);
            UpdateModifiers();
        }

        public void SwitchEffect()
        {
            var currentIndex = _effects.IndexOf(_selectedProjectile);
            for(var i = (currentIndex + 1) % _effects.Count;i != currentIndex; i = (i + 1) % _effects.Count)
                if (_effects[i].IsProjectile)
                { 
                    _selectedProjectile = _effects[i];
                    UpdateModifiers();
                    break;
                }
        }

        public EffectModifiers GetModifiers()
        {
            UpdateEffects();
            return _modifiers;
        }

        private void UpdateEffects()
        {
            _effects.RemoveAll(x => x.IsActive == false);

            if (!_selectedProjectile.IsActive)
                _selectedProjectile = _effects[0];

            foreach (var effect in _effects)
                if (!effect.IsProjectile || effect == _selectedProjectile)
                    effect.OnUpdate(Raylib.GetFrameTime());
        }

        private void UpdateModifiers()
        {
            _effects.RemoveAll(x => x.IsActive == false);

            var modifiers = new EffectModifiers
            {
                Stats = new PlayerStats(),
                Display = new PlayerDisplay() {  },
                Projectile = new PlayerProjectile()
            };

            foreach (var effect in _effects)
                if (!effect.IsProjectile || effect == _selectedProjectile)
                    effect.Apply(modifiers);

            _modifiers = modifiers;
        }
    }
}
