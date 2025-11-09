using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System.Numerics;
using GalagaFighter.Core2.GameObjects;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class HomingBehavior : ProjectileBehaviorBase
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        public HomingBehavior(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }
        public override void Update(Projectile projectile, float frameTime)
        {
            var homingState = _gameDataRegistry.Get<HomingState>(projectile);
            if (homingState.Homing == 0)
                return;
            var player = _objectService.Get<Player>(projectile.Owner);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var opponent = _objectService.GetOpponent(player);
            var finalHomingFactor = (homingState.Homing + modifiers.HomingFactor) * frameTime * 3.0f;
            var currentSpeed = projectile.Speed.Length();
            var normalizedSpeed = Vector2.Normalize(projectile.Speed);
            var homingSpeed = Vector2.Normalize(-(projectile.WorldPosition - opponent.WorldPosition));
            var finalNormalizedSpeed = Vector2.Normalize(homingSpeed * finalHomingFactor + normalizedSpeed * (1 - finalHomingFactor));
            var finalSpeed = finalNormalizedSpeed * currentSpeed;
            projectile.HurryTo(finalSpeed.X, finalSpeed.Y);
        }
    }
}
