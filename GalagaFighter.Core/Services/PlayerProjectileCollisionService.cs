using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Handlers.Collisions;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerProjectileCollisionService
    {
        void HandleCollisions();
    }

    public class PlayerProjectileCollisionService : IPlayerProjectileCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerProjectileCollisionPlanker _planker;
        private readonly ICollisionCreationService _collisionCreationService;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private readonly EdgeCollisionDetector _edgeDetector;
        private readonly ContactCollisionDetector _contactDetector;
        private readonly INearbyCollisionDetector _nearbyCollisionDetector;

        public PlayerProjectileCollisionService(IObjectService objectService, IPlayerProjectileCollisionPlanker planker,
            ICollisionCreationService collisionCreationService, IPlayerManagerFactory playerManagerFactory, INearbyCollisionDetector nearbyCollisionDetector)
        {
            _objectService = objectService;
            _planker = planker;
            _collisionCreationService = collisionCreationService;
            _playerManagerFactory = playerManagerFactory;
            _edgeDetector = new EdgeCollisionDetector();
            _contactDetector = new ContactCollisionDetector();
            _nearbyCollisionDetector = nearbyCollisionDetector;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            foreach (var player in players)
            {
                var effectManager = _playerManagerFactory.GetEffectManager(player);
                var modifiers = effectManager.GetModifiers();
                HandleCollisions(player, modifiers);
            }
        }

        private float _collisionCount = 0f;

        private void HandleCollisions(Player player, EffectModifiers modifiers)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                // Check both collision types - projectile could hit both edge AND player
                var hasEdgeCollision = _edgeDetector.HasCollision(projectile, projectile.OnNearEdgeDistance);
                if (hasEdgeCollision)  
                    projectile.OnNearEdge?.Invoke();

                var hasNearCollision = _nearbyCollisionDetector.HasCollision(player, projectile, projectile.OnNearPlayerDistance);
                if (hasNearCollision)
                    projectile.OnNearPlayer?.Invoke(player);

                var hasPlayerCollision = _contactDetector.HasCollision(player, projectile);
                if (hasPlayerCollision || hasNearCollision)
                    Collide(player, projectile, modifiers);

                if (hasPlayerCollision)
                {
                    _collisionCount += Raylib_cs.Raylib.GetFrameTime();
                    //DebugWriter.Write(_collisionCount.ToString());
                    projectile.OnCollide?.Invoke(player);
                }

            }
        }

        private void Collide(Player player, Projectile projectile, EffectModifiers modifiers)
        {
            if (modifiers.Untouchable || projectile.Modifiers.Untouchable)
                return;
            
            var effectManager = _playerManagerFactory.GetEffectManager(player);
            var effects = projectile.CreateEffects();

            foreach (var effect in effects)
                effectManager.AddEffect(effect);

            var damage = projectile.BaseDamage * projectile.Modifiers.DamageMultiplier * (1 / modifiers.Stats.Shield) * (1 / player.BaseStats.Shield);
            player.Health -= damage;

            var collisionObjects = projectile.Modifiers.OnCollide?.Invoke(player, projectile);
            if (collisionObjects != null && collisionObjects.Count > 0)
            {
                _objectService.AddRange(collisionObjects);
            }

            var planked = _planker.IsPlanked(player, projectile);
            if (planked)
            {
                _planker.HandlePlankCollision(player, projectile);
                return;
            }

            _collisionCreationService.Create(player, projectile);

            if(projectile.Modifiers.DeactivateOnCollision)
            { 
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
        }
    }
}
