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

            if (projectile.Rect.X - projectile.Rect.Width < 0f)
                projectile.IsActive = false;
            else if (projectile.Rect.X > Game.Width)
                projectile.IsActive = false;
        }
    }
}