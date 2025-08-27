using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Handlers.Collisions;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerProjectileCollisionService
    {
        void HandleCollisions(Player player, EffectModifiers modifiers);
    }

    public class PlayerProjectileCollisionService : IPlayerProjectileCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerProjectileCollisionPlanker _planker;
        private readonly ICollisionCreationService _collisionCreationService;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private readonly EdgeCollisionDetector _edgeDetector;
        private readonly ContactCollisionDetector _contactDetector;

        public PlayerProjectileCollisionService(IObjectService objectService, IPlayerProjectileCollisionPlanker planker,
            ICollisionCreationService collisionCreationService, IPlayerManagerFactory playerManagerFactory)
        {
            _objectService = objectService;
            _planker = planker;
            _collisionCreationService = collisionCreationService;
            _playerManagerFactory = playerManagerFactory;
            _edgeDetector = new EdgeCollisionDetector();
            _contactDetector = new ContactCollisionDetector();
        }

        public void HandleCollisions(Player player, EffectModifiers modifiers)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                // Check both collision types - projectile could hit both edge AND player
                var hasEdgeCollision = _edgeDetector.HasCollision(projectile);
                var hasPlayerCollision = _contactDetector.HasCollision(player, projectile);
                if (hasEdgeCollision || hasPlayerCollision)
                {
                    Collide(player, projectile, modifiers);
                }
            }
        }

        private void Collide(Player player, Projectile projectile, EffectModifiers modifiers)
        {
            if (modifiers.Untouchable)
                return;
            
            var effectManager = _playerManagerFactory.GetEffectManager(player);
            var effects = projectile.CreateEffects();

            foreach (var effect in effects)
                effectManager.AddEffect(effect);


            player.Health -= projectile.BaseDamage * projectile.Modifiers.DamageMultiplier * (1 / modifiers.Stats.Shield)*(1/player.BaseStats.Shield);

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
