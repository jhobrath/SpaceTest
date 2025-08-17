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
    public interface IProjectileCollisionService
    {
        void HandleCollisions();
    }

    public class ProjectileCollisionService : IProjectileCollisionService
    {
        private readonly IObjectService _objectService;

        public ProjectileCollisionService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                foreach(var player in players)
                {
                    if (projectile.Owner == player.Id)
                        continue;

                    if(Raylib.CheckCollisionRecs(player.Rect, projectile.Rect))
                        player.Collide(projectile);
                }
            }
        }
    }
}
