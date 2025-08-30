using GalagaFighter.Core.Handlers.Projectiles;
using GalagaFighter.Core.Models.Projectiles;
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
            var originalLifeTime = (Math.Max(Math.Abs(projectile.Speed.X)* projectile.Modifiers.SpeedMultiplier, 1020f) / 1020f) * projectile.Lifetime;
            projectile.Lifetime += frameTime;
            var actualLifeTime = (Math.Max(Math.Abs(projectile.Speed.X) * projectile.Modifiers.SpeedMultiplier, 1020f) / 1020f) * projectile.Lifetime; 

            foreach(var phaseList in projectile.Modifiers.Phases)
                for(var i = 0; i < phaseList.Value.Count;i++)
                    if(originalLifeTime < phaseList.Value[i] && actualLifeTime >= phaseList.Value[i])
                        if (projectile.Modifiers.OnPhaseChange.TryGetValue(phaseList.Key, out var func))
                            func?.Invoke(projectile, i + 1);
        }

        private void Deactivate(Projectile projectile)
        {
            var reachedEdgeX = (projectile.CurrentFrameRect.X + projectile.CurrentFrameRect.Width < 0f)
                || (projectile.CurrentFrameRect.X > Game.Width);
            var reachedEdgeY = (projectile.CurrentFrameRect.Y + projectile.CurrentFrameRect.Height < 0f)
                || (projectile.CurrentFrameRect.Y > Game.Height);

            if (reachedEdgeX && projectile.Modifiers.CanRicochet)
            {
                RicochetX(projectile);
                return;
            }

            if(reachedEdgeY && projectile.Modifiers.CanRicochet)
            {
                RicochetY(projectile);
                return;
            }
         
            if(reachedEdgeX || reachedEdgeY)
            { 
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
        }

        private void RicochetX(Projectile projectile)
        {
            projectile.Hurry(x: -1f);
            projectile.Modifiers.CanRicochet = Game.Random.NextDouble() < .5f;
        }

        private void RicochetY(Projectile projectile)
        {
            projectile.Hurry(y: -1f);
            projectile.Modifiers.CanRicochet = Game.Random.NextDouble() < .333f;
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