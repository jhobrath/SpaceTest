using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.Effects;
using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Players;

namespace GalagaFigther.Core2.Handlers.Players
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
            var effects = _gameDataRegistry.Get<List<PlayerEffect>>(player);
            RecalculateEffectsIfNecessary(player, effects);

            foreach(var effect in effects)
                effect.Update(frameTime);
        }

        private void RecalculateEffectsIfNecessary(Player player, List<PlayerEffect> effects)
        {
            if (_gameDataRegistry.Has<PlayerModifiers>(player) && effects.All(x => x.IsActive))
                return;
         
            var modifiers = new PlayerModifiers();
            
            effects.RemoveAll(x => !x.IsActive);
            foreach (var effect in effects)
                effect.Apply(modifiers);

            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            modifiers.Decorations.ForEach(x => x.InitialRotation = rotationData.InitialRotation);

            _gameDataRegistry.Set(player, modifiers);
        }
    }
}
