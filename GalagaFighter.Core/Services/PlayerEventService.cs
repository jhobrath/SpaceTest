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
        private IPlayerEffectManager _effectManager;

        private Guid _powerUpCollectedSubId;
        private Guid _effectDeactivatedSubId;
        private Guid _projectileCollidedSubId;

        public PlayerEventService(IEventService eventService, IObjectService objectService, IInputService inputService, IPlayerEffectManager effectManager)
        {
            _objectService = objectService;
            _eventService = eventService;
            _inputService = inputService;
            _effectManager = effectManager;

            Initialize();
        }

        public void Initialize()
        {
            _effectDeactivatedSubId = SubscribeEffectDeactivated();
            _projectileCollidedSubId = SubscribeProjectileCollided();
            _powerUpCollectedSubId = SubscribePowerUpCollected();
        }

        private Guid SubscribePowerUpCollected()
        {
            return _eventService.Subscribe<PowerUpCollectedEventArgs>(HandlePowerUpCollected);
        }

        private void HandlePowerUpCollected(PowerUpCollectedEventArgs args)
        {
            var effects = args.PowerUp.CreateEffects(_eventService, _objectService, _inputService);
            foreach (var effect in effects)
                _effectManager.AddEffect(args.Player, effect);
        }

        private Guid SubscribeEffectDeactivated()
        { 
            return _eventService.Subscribe<EffectDeactivatedEventArgs>(HandleEffectDeactivated); 
        }
        private void HandleEffectDeactivated(EffectDeactivatedEventArgs e)
        {
            _effectManager.RemoveEffect(e.Player, e.Effect);
        }

        private Guid SubscribeProjectileCollided() 
        { 
            return _eventService.Subscribe<ProjectileCollidedEventArgs>(HandleProjectileCollided); 
        }
        private void HandleProjectileCollided(ProjectileCollidedEventArgs e)
        {
            foreach(var effect in effects)
                _effectManager.AddEffect(e.Player, effect);
        }
    }
}
