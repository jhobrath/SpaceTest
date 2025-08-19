using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerEventService
    {
    }
    public class PlayerEventService
    {
        private IEventService _eventService;
        private Player _player1;
        private Player _player2;

        public PlayerEventService(IEventService eventService, Player player1, Player player2)
        {
            _eventService = eventService;
            _player1 = player1;
            _player2 = player2;

            Initialize();
        }

        private void Initialize()
        {
            SubscribeEffectLoss<WoodShotEffect>();
        }

        private void SubscribeEffectLoss<T>()
            where T : PlayerEffect
        {
            _eventService.Subscribe<EffectDeactivatedEventArgs<T>>(e =>
            {
                var player = _player1.Id == e.Player.Id ? _player1 : _player2;

                var effects = player.GetEffects(e.Effect);
                var selected = player.GetSelectedProjectile();
                var defaultProjectile = effects[0];

                for (var i = effects.Count - 1; i >= 0; i--)
                {
                    if (!effects[i].IsActive)
                    {
                        if (selected == effects[i])
                            player.SetSelectedProjectile(defaultProjectile);

                        effects.RemoveAt(i);
                    }
                }
            });
        }
    }
}
