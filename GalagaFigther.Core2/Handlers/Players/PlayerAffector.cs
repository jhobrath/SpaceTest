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
        private readonly IObjectService _objectService;

        public PlayerAffector(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
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

            effects.RequireRerolling = false;
        }

        private void RecalculateModifiers(Player player, PlayerEffects effects)
        {
            ClearGuns(player);

            var modifiers = new PlayerModifiers();
            
            effects.RemoveAll(x => !x.IsActive);
            foreach (var effect in effects)
                effect.Apply(modifiers);

            SetGuns(player, modifiers);

            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            foreach (var deco in modifiers.Decorations)
            {
                if (!deco.MaintainRotation)
                    deco.InitialRotation = rotationData.InitialRotation;
            }

            modifiers.EffectCount = effects.Count;

            _gameDataRegistry.Set(player, modifiers);
        }

        private void ClearGuns(Player player)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            modifiers.Guns.ForEach(x => x.IsActive = false);
            modifiers.Guns.Clear();
        }

        private void SetGuns(Player player, PlayerModifiers modifiers)
        {
            foreach(var effect in modifiers.CreateGuns)
            {
                foreach(var gun in effect.Value(player))
                {
                    modifiers.Guns.Add(gun);
                    _objectService.Add(gun);
                }
            }
        }
    }
}
