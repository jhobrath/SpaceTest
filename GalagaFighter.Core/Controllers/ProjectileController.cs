using GalagaFighter.Core.Handlers.Projectiles;
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
        private IProjectileSplitter _projectileSplitter;

        public ProjectileController(IProjectileMover projectileMover, IProjectileRotator projectileRotator,
            IProjectileSplitter projectileSplitter)
        {
            _projectileMover = projectileMover;
            _projectileRotator = projectileRotator;
            _projectileSplitter = projectileSplitter;
        }

        //Each projectile should have its own controller instance
        public IProjectileController Create()
        {
            return new ProjectileController(_projectileMover.Create(), _projectileRotator.Create(), _projectileSplitter);
        }

        public void Update(Game game, Projectile projectile)
        {

            var frameTime = Raylib.GetFrameTime();

            SetPhase(projectile, frameTime);
            SetSprite(projectile, frameTime);

            _projectileMover.Move(projectile);
            _projectileRotator.Rotate(projectile);
            _projectileSplitter.Split(projectile, Create());

            SetColor(projectile);
            SetSize(projectile);
            Deactivate(projectile);
        }

        private void SetColor(Projectile projectile)
        {
            projectile.Color = Color.White;

            if (projectile.Modifiers.RedAlpha != 1f)
                projectile.Color = projectile.Color.ApplyRed(projectile.Modifiers.RedAlpha);
            if (projectile.Modifiers.GreenAlpha != 1f)
                projectile.Color = projectile.Color.ApplyGreen(projectile.Modifiers.GreenAlpha);
            if (projectile.Modifiers.BlueAlpha != 1f)
                projectile.Color = projectile.Color.ApplyBlue(projectile.Modifiers.BlueAlpha);
            if (projectile.Modifiers.Opacity != 1f)
                projectile.Color = projectile.Color.ApplyAlpha(projectile.Modifiers.Opacity);
        }

        private void SetSprite(Projectile projectile, float frameTime)
        {
            projectile.Sprite = projectile.Modifiers.Sprite ?? projectile.Sprite;
            projectile.Sprite.Update(frameTime);
            projectile.Modifiers.OnSpriteUpdate?.Invoke(projectile);
        }

        private void SetPhase(Projectile projectile, float frameTime)
        {
            var originalLifeTime = projectile.Lifetime;
            projectile.Lifetime += frameTime;

            if (projectile.Modifiers.Phases == null)
                return;

            for(var i = 0; i < projectile.Modifiers.Phases.Count;i++)
            {
                if(originalLifeTime < projectile.Modifiers.Phases[i] && projectile.Lifetime >= projectile.Modifiers.Phases[i])
                {
                    projectile.Modifiers.OnPhaseChange?.Invoke(projectile, i+1);
                }
            }
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
            var currentFrameSizeX = projectile.Rect.Width * projectile.Modifiers.SizeMultiplier.X;
            var currentFrameSizeY = projectile.Rect.Height * projectile.Modifiers.SizeMultiplier.Y;
            projectile.CurrentFrameRect = new Rectangle(
                projectile.Rect.X - (currentFrameSizeX - projectile.Rect.Width) / 2,
                projectile.Rect.Y - (currentFrameSizeY - projectile.Rect.Height) / 2,
                currentFrameSizeX,
                currentFrameSizeY
            );
        }
    }
}