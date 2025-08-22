using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Controllers
{
    public interface IProjectileController
    {
        IProjectileController Create();
        void Update(Game game, Projectile projectile);
    }

    public class ProjectileController : IProjectileController
    {
        private IProjectileMover _projectileMover;

        public ProjectileController(IProjectileMover projectileMover)
        {
            _projectileMover = projectileMover;
        }

        //Each projectile should have its own controller instance
        public IProjectileController Create()
        {
            return new ProjectileController(_projectileMover.Create());
        }

        public void Update(Game game, Projectile projectile)
        {
            _projectileMover.Move(projectile);

            if (projectile.Modifiers.PlankDuration > 0f)
                return;

            if (projectile.Rect.X < 0f)
            { 
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
            else if (projectile.Rect.X + projectile.Rect.Width > Game.Width)
            { 
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
        }
    }
}