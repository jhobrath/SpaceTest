using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;

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
        private IProjectileRotator _projectileRotator;

        public ProjectileController(IProjectileMover projectileMover, IProjectileRotator projectileRotator)
        {
            _projectileMover = projectileMover;
            _projectileRotator = projectileRotator;
        }

        //Each projectile should have its own controller instance
        public IProjectileController Create()
        {
            return new ProjectileController(_projectileMover.Create(), _projectileRotator.Create());
        }

        public void Update(Game game, Projectile projectile)
        {

            var frameTime = Raylib.GetFrameTime();
            projectile.Sprite.Update(frameTime);
            projectile.Modifiers.OnSpriteUpdate?.Invoke(projectile);


            _projectileMover.Move(projectile);
            _projectileRotator.Rotate(projectile);

            SetSize(projectile);
            Deactivate(projectile);
        }

        private void Deactivate(Projectile projectile)
        {
            if (projectile.CurrentFrameRect.X + projectile.CurrentFrameRect.Width < 0f)
            {
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
            else if (projectile.CurrentFrameRect.X > Game.Width)
            {
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
        }

        private void SetSize(Projectile projectile)
        {
            var currentFrameSizeX = projectile.Rect.Width * projectile.Modifiers.SizeMultiplier;
            var currentFrameSizeY = projectile.Rect.Height * projectile.Modifiers.SizeMultiplier;
            projectile.CurrentFrameRect = new Rectangle(
                projectile.Rect.X - (currentFrameSizeX - projectile.Rect.Width) / 2,
                projectile.Rect.Y - (currentFrameSizeY - projectile.Rect.Height) / 2,
                currentFrameSizeX,
                currentFrameSizeY
            );
        }
    }
}