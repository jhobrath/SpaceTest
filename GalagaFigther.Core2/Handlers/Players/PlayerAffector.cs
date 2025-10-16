using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerAffector
    {
        void Affect(Player player, float frameTime);
    }
    public class PlayerAffector : IPlayerAffector
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerAffector(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Affect(Player player, float frameTime)
        {
            var effects = _gameDataRegistry.Get<PlayerEffects>(player);
            RecalculateEffectsIfNecessary(player, effects);

            foreach(var effect in effects)
                effect.Update(frameTime);
        }

        private void RecalculateEffectsIfNecessary(Player player, PlayerEffects effects)
        {
            if (!_gameDataRegistry.Has<PlayerModifiers>(player))
                RecalculateModifiers(player, effects);

            else if (effects.Any(x => !x.IsActive))
                RecalculateModifiers(player, effects);

            else if(effects.RequireRerolling)
                RecalculateModifiers(player, effects);
        }

        private void RecalculateModifiers(Player player, PlayerEffects effects)
        {
            var modifiers = new PlayerModifiers();
            
            effects.RemoveAll(x => !x.IsActive);
            foreach (var effect in effects)
                effect.Apply(modifiers);

            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            foreach (var deco in modifiers.Decorations)
            {
                if (!deco.MaintainRotation)
                    deco.InitialRotation = rotationData.InitialRotation;
            }

            modifiers.EffectCount = effects.Count;

            _gameDataRegistry.Set(player, modifiers);
        }
    }
}
