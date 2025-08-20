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
                if (projectile.EffectsApplied)
                    continue;

                foreach(var player in players)
                {
                    if (projectile.Owner == player.Id)
                        continue;

                    if(Raylib.CheckCollisionRecs(player.Rect, projectile.Rect))
                    { 
                        var collisionBehavior = _playerEffectManager?.GetSelectedProjectileEffect(player)?.CollisionBehavior
                            ?? _playerEffectManager?.GetProjectileEffects(player).FirstOrDefault()?.CollisionBehavior;
                        collisionBehavior?.Apply(player, projectile);

                        player.Collide(projectile);
                        projectile.Collide(player);
                    }
                }
            }
        }
    }
}
