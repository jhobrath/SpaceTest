using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileUpdater
    {
        void Update(Game game, Projectile projectile);
    }   
    public class ProjectileUpdater : IProjectileUpdater
    {
        public void Update(Game game, Projectile projectile)
        {
            var frameTime = Raylib.GetFrameTime();
            projectile.Move(projectile.Speed.X * frameTime, projectile.Speed.Y * frameTime);
        }
    }
}