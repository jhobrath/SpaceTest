using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System;
using System.Linq;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class EdgeDeactivatesBehavior : ProjectileBehaviorBase
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        public EdgeDeactivatesBehavior(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }
        public override void Update(Projectile projectile, float frameTime)
        {
            var screenData = _gameDataRegistry.Get<GameState>();
            if (projectile.WorldPosition.X < -50 || projectile.WorldPosition.X > screenData.ScreenSize.X + 50)
                projectile.IsActive = false;
            if (projectile.WorldPosition.Y < -50 || projectile.WorldPosition.Y > screenData.ScreenSize.Y + 50)
                projectile.IsActive = false;
            if (projectile.Lifetime != -1f)
            {
                projectile.Lifetime -= frameTime;
                if (projectile.Lifetime < 0f)
                {
                    projectile.IsActive = false;
                }
            }

            var childEmitters = _objectService.GetChildren<ParticleEmitter>(projectile).ToList();
            childEmitters.ForEach(x => x.IsActive = false);
        }
    }
}
