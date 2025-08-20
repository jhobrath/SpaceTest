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
        public override string IconPath => "Sprites/Effects/iceshot.png";
        private readonly SpriteWrapper _sprite;
        public override bool IsProjectile => true;

        private readonly IPlayerShootingBehavior? _shootingBehavior;
        public override IPlayerShootingBehavior? ShootingBehavior => _shootingBehavior;

        private readonly IObjectService _objectService;

        protected override float Duration => 10f; // 10 seconds

        public IceShotEffect(IObjectService objectService)
        {
            _objectService = objectService;
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/IceShotShip.png"), 3, .33f);
            _shootingBehavior = new IceShotShootingBehavior(objectService);
        }

        public override void OnUpdate(float frameTime)
        {
            base.OnUpdate(frameTime); // Handles duration and deactivation
            // Add any custom update logic here if needed
        }

        public override void Apply(PlayerDisplay display)
        {
            display.Sprite = _sprite;
        }
    }
}
