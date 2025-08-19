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

        public WoodShotEffect(IEventService eventService, IObjectService objectService, IInputService inputService)
        {
            _eventService = eventService;
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/WoodShotShip.png"));
            _shootingBehavior = new WoodShotShootingBehavior(_eventService, objectService, inputService);
            _eventService.Subscribe<ProjectileFiredEventArgs<WoodProjectile>>(HandleWoodShotFired);
        }

        private void HandleWoodShotFired(ProjectileFiredEventArgs<WoodProjectile> args)
        {
            _remainingBullets--;
            if(_remainingBullets == 0)
            {
                Deactivate();
                _eventService.Unsubscribe<ProjectileFiredEventArgs<WoodProjectile>>(HandleWoodShotFired);
                _eventService.Publish(new EffectDeactivatedEventArgs(this, args.Player));
            }
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Sprite = _sprite;
        }
    }
}
