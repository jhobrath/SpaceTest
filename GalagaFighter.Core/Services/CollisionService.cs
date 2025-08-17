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
    public interface ICollisionService
    {
        List<Projectile> GetProjectileCollisions(Player player, List<Projectile> projectiles);
    }
    public class CollisionService : ICollisionService
    {
        public List<Projectile> GetProjectileCollisions(Player player, List<Projectile> projectiles)
        {
            var output = new List<Projectile>();
            foreach(var projectile in projectiles)
                if (Raylib.CheckCollisionRecs(player.Rect, projectile.Rect))
                    output.Add(projectile);

            return output;
        }
    }
}
