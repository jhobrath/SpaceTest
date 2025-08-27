using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerSpender
    {
        void HandleDefensiveSpend(Player player, EffectModifiers modifiers);
        void HandleOffensiveSpend(Player player, EffectModifiers modifiers);
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

        public void HandleOffensiveSpend(Player player, EffectModifiers modifiers)
        {
            if (player.OffensiveAugment == null)
                return;

            var switchPress = _inputService.GetShoot(player.Id);
            if (!switchPress.IsDoublePressed)
                return;

            var resourceManager = _playerManagerFactory.GetResourceManager(player.Id);
            var didSpend = resourceManager.Spend(100f);
            if (!didSpend)
                return;

            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            effectManager.AddEffect(player.OffensiveAugment());
        }

        public void HandleDefensiveSpend(Player player, EffectModifiers modifiers)
        {
            if (player.DefensiveAugment == null)
                return;

            var switchPress = _inputService.GetSwitch(player.Id);
            if (!switchPress.IsPressed)
                return;

            var resourceManager = _playerManagerFactory.GetResourceManager(player.Id);
            var didSpend = resourceManager.Spend(10f);
            if (!didSpend)
                return;

            var effectManager = _playerManagerFactory.GetEffectManager(player.Id);
            effectManager.AddEffect(player.DefensiveAugment());
        }
    }
}
