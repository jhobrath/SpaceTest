using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.GameObjects.Collisions;
using GalagaFighter.Core2.GameObjects;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public interface IProjectilePlayerCollisionBehavior
    {
        void OnPlayerCollision(Projectile projectile, Player player, IGameDataRegistry registry, IObjectService objectService);
    }

    public class DefaultProjectilePlayerCollisionBehavior : IProjectilePlayerCollisionBehavior
    {
        public void OnPlayerCollision(Projectile projectile, Player player, IGameDataRegistry registry, IObjectService objectService)
        {
            if (player.Id == projectile.Owner)
                return;
            if (!projectile.Collidable)
                return;

            UpdateHealth(player, projectile, registry, objectService);
            UpdateEffects(player, projectile, registry);
            CreateCollision(projectile, objectService);
            DeactivateProjectile(projectile, objectService);
            AddDamageRenderEffect(player, registry);
        }

        private void AddDamageRenderEffect(Player player, IGameDataRegistry registry)
        {
            var renderEffects = registry.Get<PlayerRenderEffects>(player);
            renderEffects.Add(new PlayerRenderEffect(RenderEffectActions.Flash(Color.Red), .25f, 1));
            renderEffects.Add(new PlayerRenderEffect(RenderEffectActions.Heartbeat, .125f, -.0625f));
        }

        private void DeactivateProjectile(Projectile projectile, IObjectService objectService)
        {
            if (projectile.DestroyOnHit)
                projectile.IsActive = false;
            else
                projectile.Collidable = false;
            var emitters = objectService.GetChildren<ParticleEmitter>(projectile).ToList();
            emitters.ForEach(x => x.IsActive = false);
        }

        private void CreateCollision(Projectile projectile, IObjectService objectService)
        {
            var collisionPoint = new Vector2(
                projectile.Speed.X < 0 ? projectile.WorldPosition.X : projectile.WorldPosition.X + projectile.Width,
                projectile.WorldPosition.Y + projectile.Height / 2f);
            var collision = new DefaultCollision(collisionPoint, 55f);
            objectService.Add(collision);
        }

        private void UpdateEffects(Player player, Projectile projectile, IGameDataRegistry registry)
        {
            var currentEffects = registry.Get<PlayerEffects>(player);
            var effects = projectile.CreateEffects(player);
            currentEffects.AddRange(effects);
        }

        private void UpdateHealth(Player player, Projectile projectile, IGameDataRegistry registry, IObjectService objectService)
        {
            var opponent = objectService.GetOpponent(player);
            var opponentStats = registry.Get<PlayerBaseStats>(opponent);
            var opponentModifiers = registry.Get<PlayerModifiers>(opponent);
            var shooterDamageMultiplier = opponentStats.Damage * opponentModifiers.Stats.DamageMultiplier;

            var baseStats = registry.Get<PlayerBaseStats>(player);
            var modifiers = registry.Get<PlayerModifiers>(player);
            var shieldMultiplier = baseStats.Shield * modifiers.Stats.ShieldMultiplier;

            var damage = projectile.Damage * shooterDamageMultiplier * (1 / shieldMultiplier);
            player.Health -= damage;
        }
    }
}
