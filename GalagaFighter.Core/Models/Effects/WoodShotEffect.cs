using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Players;
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
        private readonly SpriteWrapper _sprite;
        public override bool IsProjectile => true;

        private readonly IPlayerShootingBehavior? _shootingBehavior;
        public override IPlayerShootingBehavior? ShootingBehavior => _shootingBehavior;

        public WoodShotEffect(IObjectService objectService, IInputService inputService)
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/WoodShotShip.png"));
            _shootingBehavior = new WoodShotShootingBehavior(objectService, inputService);
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Sprite = _sprite;
        }
    }
}
