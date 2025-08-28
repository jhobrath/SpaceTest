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
        }

        public void AddEffect(PlayerEffect newEffect)
        {
            _effects.Add(newEffect);

            ReplaceExistingEffect(newEffect);
            LimitEffectCount(newEffect);
            UpdateModifiers();
        }

        private void ReplaceExistingEffect(PlayerEffect newEffect)
        {
            for(var i = _effects.Count - 2;i >= 0;i--)
            {
                if (!_effects[i].IsProjectile)
                    continue;

                if (_effects[i].GetType() == newEffect.GetType())
                {
                    _effects[i] = newEffect;
                    _effects.RemoveAt(_effects.Count - 1);
                }
            }

            if (_effects.All(x => x != _selectedProjectile))
                _selectedProjectile = newEffect;
        }

        private void LimitEffectCount(PlayerEffect newEffect)
        {
            if (newEffect.MaxCount == 0)
                return;

            var effectType = newEffect.GetType();
            var ofType = _effects.Count(x => x.GetType() == effectType);
            if (ofType <= newEffect.MaxCount)
                return;

            var first = _effects.First(x => x.GetType() == effectType);
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
            var effectsToRemove = _effects.Where(x => x.IsActive == false).ToList();
            if (effectsToRemove.Any())
            {
                foreach(var effect in effectsToRemove)
                    foreach(var key in effect.DecorationKeys)
                        if(_modifiers.Decorations?.ContainsKey(key) ?? false)
                            _modifiers.Decorations?.Remove(key);

                _effects.RemoveAll(x => x.IsActive == false);

                if (!_selectedProjectile.IsActive)
                    _selectedProjectile = _effects[0];
                
                UpdateModifiers();
            }


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
                Projectile = new PlayerProjectile(),
                Decorations = new SpriteDecorations()
            };

            foreach (var effect in _effects)
                if (!effect.IsProjectile || effect == _selectedProjectile)
                    effect.Apply(modifiers);

            _modifiers = modifiers;
        }
    }
}
