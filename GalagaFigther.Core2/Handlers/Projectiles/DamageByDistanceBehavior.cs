using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System.Numerics;
using GalagaFighter.Core2.GameObjects;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class DamageByDistanceBehavior : ProjectileBehaviorBase
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        public DamageByDistanceBehavior(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }
        public override void Update(Projectile projectile, float frameTime)
        {
            var state = _gameDataRegistry.Get<DamageByDistanceState>(projectile);
            if(state.SpawnPoint == null)
            {
                state.SpawnPoint = projectile.WorldPosition;
                state.OriginalDamage = projectile.Damage;
                return;
            }

            var distanceTraveled = Vector2.Distance(projectile.WorldPosition, state.SpawnPoint.Value);
            var halvingUnits = (distanceTraveled / 1000f);
            var appliedHalfLife = MathF.Pow(.5f, halvingUnits);
            projectile.Damage = state.OriginalDamage * appliedHalfLife;
        }
    }
}
