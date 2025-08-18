using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public class IceShotEffect : PlayerEffect
    {
        private readonly SpriteWrapper _sprite;
        private readonly IPlayerShootingBehavior? _shootingBehavior;
        public override IPlayerShootingBehavior? ShootingBehavior => _shootingBehavior;

        private readonly IObjectService _objectService;

        public IceShotEffect(IObjectService objectService)
        {
            _objectService = objectService;
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/IceShotShip.png"), 3, .33f);
            _shootingBehavior = new IceShotShootingBehavior(objectService);
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Sprite = _sprite;
        }
    }
}
