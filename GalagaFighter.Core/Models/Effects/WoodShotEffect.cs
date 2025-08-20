using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class WoodShotEffect : PlayerEffect
    {
        private readonly IEventService _eventService;
        private readonly SpriteWrapper _sprite;
        public override bool IsProjectile => true;
        public override string IconPath => "Sprites/Effects/woodshot.png";

        private readonly IPlayerShootingBehavior? _shootingBehavior;
        public override IPlayerShootingBehavior? ShootingBehavior => _shootingBehavior;

        private int _remainingBullets = 3;
        private Guid _ownerId;
        private Guid _subscriptionId;

        public WoodShotEffect(IEventService eventService, IObjectService objectService, IInputService inputService)
        {
            _eventService = eventService;
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/WoodShotShip.png"));
            _shootingBehavior = new WoodShotShootingBehavior(_eventService, objectService, inputService);
            _subscriptionId = _eventService.Subscribe<ProjectileFiredEventArgs<WoodProjectile>>(HandleWoodShotFired);
        }

        public void SetOwner(Guid ownerId)
        {
            _ownerId = ownerId;
        }

        private void HandleWoodShotFired(ProjectileFiredEventArgs<WoodProjectile> args)
        {
            // Only decrement if the event is for the player who owns this effect
            if (args.Player == null || args.Player.Id != _ownerId)
                return;

            _remainingBullets--;
            if(_remainingBullets == 0)
            {
                Deactivate();
                _eventService.Unsubscribe<ProjectileFiredEventArgs<WoodProjectile>>(_subscriptionId);
                _eventService.Publish(new EffectDeactivatedEventArgs(this, args.Player));
            }
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Sprite = _sprite;
        }
    }
}
