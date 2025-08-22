using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileUpdater
    {
        IProjectileUpdater Create();
        void Update(Game game, Projectile projectile);
    }   
    public class ProjectileUpdater : IProjectileUpdater
    {
        private IProjectileMover _projectileMover;

        public ProjectileUpdater(IProjectileMover projectileMover)
        {
            _projectileMover = projectileMover;
        }

        //Each projectile should have its own updater instance
        public IProjectileUpdater Create()
        {
            return new ProjectileUpdater(_projectileMover.Create());
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