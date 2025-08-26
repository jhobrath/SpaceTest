using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerSpender
    {
        void Spend(Player player, EffectModifiers modifiers);
    }
    public class PlayerSpender : IPlayerSpender
    {
        private readonly IInputService _inputService;
        private readonly IPlayerManagerFactory _playerManagerFactory;

        public PlayerSpender(IInputService inputService, IPlayerManagerFactory playerManagerFactory)
        {
            _playerManagerFactory = playerManagerFactory;
            _inputService = inputService;
        }

        public void Spend(Player player, EffectModifiers modifiers)
        {
            var switchPress = _inputService.GetSwitch(player.Id);
            if (!switchPress.IsPressed)
                return;

            var resourceManager = _playerManagerFactory.GetResourceManager(player.Id);
            var didSpend = resourceManager.Spend(10f);
            if (!didSpend)
                return;

            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            effectManager.AddEffect(new DefensiveDuckEffect());
        }
    }
}
