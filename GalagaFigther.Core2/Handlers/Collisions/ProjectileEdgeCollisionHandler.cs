using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Services;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.Collisions
{
    public interface IProjectileEdgeCollisionHandler
    {
        void Handle(Projectile projectile);
    }

    public class ProjectileEdgeCollisionHandler : IProjectileEdgeCollisionHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly Dictionary<Type, IProjectileBehavior> _edgeBehaviors;

        public ProjectileEdgeCollisionHandler(IGameDataRegistry gameDataRegistry, IObjectService objectService, IEnumerable<IProjectileBehavior> edgeBehaviors)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _edgeBehaviors = edgeBehaviors.ToDictionary(b => b.GetType());
        }

        public void Handle(Projectile projectile)
        {
            var screenData = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Game.GameState>();
            if (projectile.WorldPosition.X < -50 || projectile.WorldPosition.X > screenData.ScreenSize.X + 50 ||
                projectile.WorldPosition.Y < -50 || projectile.WorldPosition.Y > screenData.ScreenSize.Y + 50)
            {
                if (projectile.EdgeCollisionHandler != null &&
                    _edgeBehaviors.TryGetValue(projectile.EdgeCollisionHandler, out var behavior))
                {
                    behavior.Update(projectile, 0f); // Only handle deactivation/emitters
                }
            }
        }
    }
}
