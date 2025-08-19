using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using System.Linq;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerEventService
    {
    }
    public class PlayerEffectService
    {
        private IEventService _eventService;

        public PlayerEffectService(IEventService eventService)
        {
            _eventService = eventService;

            Initialize();
        }

        private void Initialize()
        {
            SubscribeEffectDeactivated<DefaultShootEffect>();
            SubscribeEffectDeactivated<IceShotEffect>();
            SubscribeEffectDeactivated<FireRateEffect>();
            SubscribeEffectDeactivated<WoodShotEffect>();
            SubscribeEffectDeactivated<FrozenEffect>();

            SubscribeEffectActivated<DefaultShootEffect>();
            SubscribeEffectActivated<IceShotEffect>();
            SubscribeEffectActivated<FireRateEffect>();
            SubscribeEffectActivated<WoodShotEffect>();
            SubscribeEffectActivated<FrozenEffect>();
        }

        private void SubscribeEffectDeactivated<T>() where T : PlayerEffect 
        { 
            _eventService.Subscribe<EffectDeactivatedEventArgs<T>>(HandleEffectDeactivated); 
        }
        private static void HandleEffectDeactivated<T>(EffectDeactivatedEventArgs<T> e) where T : PlayerEffect
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

        private void SubscribeEffectActivated<T>() where T : PlayerEffect { _eventService.Subscribe<EffectActivatedEventArgs<T>>(HandleEffectActivated); }
        private static void HandleEffectActivated<T>(EffectActivatedEventArgs<T> e) where T : PlayerEffect
        {
            if (!e.Effect.IsProjectile)
            {
                e.Player.StatusEffects.Add(e.Effect);
                return;
            }
         
            var duplicates = e.Player.ProjectileEffects.Where(x => x.GetType() == e.Effect.GetType()).ToList();
            if (e.Player.SelectedProjectileEffect != null && duplicates.Contains(e.Player.SelectedProjectileEffect))
                e.Player.SelectedProjectileEffect = e.Effect;

            foreach (var duplicate in duplicates)
                e.Player.ProjectileEffects.Remove(duplicate);

            e.Player.ProjectileEffects.Add(e.Effect);
        }
    }
}
