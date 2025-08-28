using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Static;
using Raylib_cs;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IProjectileMoverPlanker
    {
        IProjectileMoverPlanker Create();
        void Plank(Projectile projectile);
    }
    public class ProjectileMoverPlanker : IProjectileMoverPlanker
    {
        private bool _planked = false;
        private float _plankTime = 0f;

        public IProjectileMoverPlanker Create()
        {
            return new ProjectileMoverPlanker();
        }

        public void Plank(Projectile projectile)
        {
            if (!_planked)
                CheckShouldPlank(projectile);

            if (!_planked)
                return;

            _plankTime += Raylib.GetFrameTime();
            if (_plankTime > projectile.Modifiers.PlankDuration)
            {
                projectile.IsActive = false;
            }
        }

        private void CheckShouldPlank(Projectile projectile)
        {
            if (projectile.Speed.X < 0 && projectile.Rect.X <= 0)
            {
                AudioService.PlayWallStickSound();
                projectile.MoveTo(x: 0f);
                projectile.HurryTo(x: 0f, y: 0f);
                _planked = true;
            }
            else if (projectile.Speed.X > 0 && projectile.Rect.X + projectile.Rect.Width >= Game.Width)
            {
                AudioService.PlayWallStickSound();
                projectile.MoveTo(x: Game.Width - projectile.Rect.Width);
                projectile.HurryTo(x: 0f, y: 0f);
                _planked = true;
            }
        }
    }
}
