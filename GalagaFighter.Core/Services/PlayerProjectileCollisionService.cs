using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerProjectileCollisionService
    {
        void HandleCollisions();
    }

    public class PlayerProjectileCollisionService : IPlayerProjectileCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerEffectManager _playerEffectManager;

        public PlayerProjectileCollisionService(IObjectService objectService, IPlayerEffectManager playerEffectManager)
        {
            _objectService = objectService;
            _playerEffectManager = playerEffectManager;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                var target = players.Single(x => projectile.Owner != x.Id);

                if (!Raylib.CheckCollisionRecs(target.Rect, projectile.Rect))
                    continue;

                Collide(target, projectile);
            }
        }

        private void Collide(Player player, Projectile projectile)
        {
            var effects = projectile.CreateEffects();
            foreach (var effect in effects)
                player.Effects.AddRange(effects);

            player.Health -= projectile.BaseDamage;

            projectile.IsActive = false;
        }
    }
}
