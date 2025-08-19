using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using System;
using System.Linq;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerEventService
    {
        void Initialize();
    }
    public class PlayerEventService : IPlayerEventService
    {
        private IObjectService _objectService;
        private IEventService _eventService;
        private IInputService _inputService;

        public PlayerEventService(IEventService eventService, IObjectService objectService, IInputService inputService)
        {
            _objectService = objectService;
            _eventService = eventService;
            _inputService = inputService;

            Initialize();
        }

        public void Initialize()
        {
            SubscribeEffectDeactivated();
            SubscribeProjectileCollided();
            SubscribePowerUpCollected();
        }

        private void SubscribePowerUpCollected()
        {
            _eventService.Subscribe<PowerUpCollectedEventArgs>(HandlePowerUpCollected);
        }

        private void HandlePowerUpCollected(PowerUpCollectedEventArgs args)
        {
            var effects = args.PowerUp.CreateEffects(_eventService, _objectService, _inputService);
            foreach (var effect in effects)
                AddEffect(args.Player, effect);
        }

        private void SubscribeEffectDeactivated()
        { 
            _eventService.Subscribe<EffectDeactivatedEventArgs>(HandleEffectDeactivated); 
        }
        private static void HandleEffectDeactivated(EffectDeactivatedEventArgs e)
        {
            if (!e.Effect.IsProjectile)
            {
                e.Player.StatusEffects.Remove(e.Effect);
                return;
            }

            if (e.Player.SelectedProjectileEffect == e.Effect)
                e.Player.SelectedProjectileEffect = e.Player.ProjectileEffects[0];

            e.Player.ProjectileEffects.Remove(e.Effect);
        }

        private void SubscribeProjectileCollided() 
        { 
            _eventService.Subscribe<ProjectileCollidedEventArgs>(HandleProjectileCollided); 
        }
        private void HandleProjectileCollided(ProjectileCollidedEventArgs e)
        {
            var effects = e.Projectile.CreateEffects(_objectService);
            foreach(var effect in effects)
                AddEffect(e.Player, effect);
        }

        private void AddEffect(Player player, PlayerEffect effect)
        {
            if (!effect.IsProjectile)
            {
                player.StatusEffects.Add(effect);
                return;
            }

            var duplicates = player.ProjectileEffects.Where(x => x.GetType() == effect.GetType()).ToList();
            if (player.SelectedProjectileEffect != null && duplicates.Contains(player.SelectedProjectileEffect))
                player.SelectedProjectileEffect = effect;

            foreach (var duplicate in duplicates)
                player.ProjectileEffects.Remove(duplicate);

            player.ProjectileEffects.Add(effect);
        }
    }
}
