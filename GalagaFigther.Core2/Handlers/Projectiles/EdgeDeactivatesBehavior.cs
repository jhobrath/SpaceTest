using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using System;
using System.Linq;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class DefaultProjectileEdgeCollisionBehavior : ProjectileBehaviorBase
    {
        private readonly IObjectService _objectService;
        public DefaultProjectileEdgeCollisionBehavior(IObjectService objectService)
        {
            _objectService = objectService;
        }
        public override void Update(Projectile projectile, float frameTime)
        {
            projectile.IsActive = false;

            var childEmitters = _objectService.GetChildren<ParticleEmitter>(projectile).ToList();
            childEmitters.ForEach(x => x.IsActive = false);
        }
    }
}
