using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Events;
using System.Numerics;

namespace GalagaFighter.Core.Controllers
{
    public class PlayerController
    {
        private readonly IPlayerEffectManager _effectManager;
        private readonly IInputService _inputService;
        private readonly IEventService _eventService;

        public PlayerController(IPlayerEffectManager effectManager, IInputService inputService, IEventService eventService)
        {
            _effectManager = effectManager;
            _inputService = inputService;
            _eventService = eventService;
        }

        public void Update(Player player, Game game)
        {
            var stats = new PlayerStats();
            var display = new PlayerDisplay(player.Display.Sprite, player.Display.Rect, player.Display.Rotation);

            var projectileEffects = _effectManager.GetProjectileEffects(player);
            var statusEffects = _effectManager.GetStatusEffects(player);
            var selectedProjectileEffect = _effectManager.GetSelectedProjectileEffect(player) ?? (projectileEffects.Count > 0 ? projectileEffects[0] : null);

            IPlayerInputBehavior? inputBehavior = projectileEffects.Count > 0 ? projectileEffects[0].InputBehavior : null;
            IPlayerMovementBehavior? movementBehavior = projectileEffects.Count > 0 ? projectileEffects[0].MovementBehavior : null;
            IPlayerShootingBehavior? shootingBehavior = projectileEffects.Count > 0 ? projectileEffects[0].ShootingBehavior : null;

            void apply(PlayerEffect effect)
            {
                effect.Apply(stats);
                effect.Apply(display);
                if (effect.InputBehavior != null) inputBehavior = effect.InputBehavior;
                if (effect.MovementBehavior != null) movementBehavior = effect.MovementBehavior;
                if (effect.ShootingBehavior != null) shootingBehavior = effect.ShootingBehavior;
            }

            foreach (var effect in statusEffects)
                apply(effect);

            if (selectedProjectileEffect != null)
                apply(selectedProjectileEffect);

            player.Stats = stats;

            var input = inputBehavior?.Apply(player);
            var movement = movementBehavior?.Apply(player, input);
            shootingBehavior?.Apply(player, input, movement);

            if (input?.Switch?.IsPressed == true)
                _effectManager.SwitchProjectileEffect(player);

            player.Display = display;
        }

        public void TakeDamage(Player player, int damage)
        {
            float oldHealth = player.Health;
            player.Health -= damage;
            _eventService.Publish(new PlayerHealthChangedEventArgs(player, oldHealth, player.Health));
        }
    }
}
