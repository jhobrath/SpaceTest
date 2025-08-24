using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Effects;
using System.Collections.Generic;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerSwitcher
    {
        void Switch(Player player, EffectModifiers modifiers);
    }
    public class PlayerSwitcher : IPlayerSwitcher
    {
        private IInputService _inputService;

        public PlayerSwitcher(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void Switch(Player player, EffectModifiers modifiers)
        {
            player.Effects.RemoveAll(x => x.IsActive == false);
            if (!player.SelectedProjectile.IsActive)
                player.SelectedProjectile = player.Effects[0];

            var switchButton = _inputService.GetSwitch(player.Id);
            if (!switchButton.IsPressed)
                return;

            var projectileEffects = new List<PlayerEffect>();

            foreach (var effect in player.Effects)
            {
                if (effect.IsProjectile)
                    projectileEffects.Add(effect);
            }

            if (projectileEffects.Count == 0)
                return;

            var currentIndex = projectileEffects.IndexOf(player.SelectedProjectile);
            var nextIndex = (currentIndex + 1) % projectileEffects.Count;
            player.SelectedProjectile = projectileEffects[nextIndex];

            if (player.SelectedProjectile is MagnetEffect)
            {
                var shoot = _inputService.GetShoot(player.Id);
                if (shoot)
                    AudioService.PlayMagnetSound();
            }
        }
    }
}
