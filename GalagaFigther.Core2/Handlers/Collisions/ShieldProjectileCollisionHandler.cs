using GalagaFighter.Core2.GameObjects.Shields;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Helpers;

namespace GalagaFighter.Core2.Handlers.Collisions
{
    public interface IProjectileShieldCollisionHandler
    {
        void Handle(Projectile projectile, ShieldPixel shieldPixel);
    }
    public class ProjectileShieldCollisionHandler : IProjectileShieldCollisionHandler
    {
        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public ProjectileShieldCollisionHandler(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Handle(Projectile projectile, ShieldPixel shieldPixel)
        {
            if (shieldPixel.Owner == projectile.Owner)
                return; 

            var player = _objectService.GetPlayer(projectile);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var damage = projectile.Damage * modifiers.Stats.DamageMultiplier;
            shieldPixel.Health -= damage;
            shieldPixel.Color = shieldPixel.Color.ApplyAlpha(shieldPixel.Health / 15f);
            if (shieldPixel.Health <= 0f)
                shieldPixel.IsActive = false;

            if(!projectile.DestroyOnHit)
                projectile.NeedsDeactivating = true;

            projectile.Collidable = false;
        }
    }
}
