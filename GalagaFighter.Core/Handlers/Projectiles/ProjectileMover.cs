using GalagaFighter.Core.Handlers.Projectiles;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IProjectileMover
    {
        IProjectileMover Create();
        void Move(Projectile projectile);
    }
    public class ProjectileMover : IProjectileMover
    {
        private IProjectileMoverWindUpper _projectileMoverWindUpper;
        private IProjectileMoverPlanker _projectileMoverPlanker;
        private IProjectileRotator _projectileRotator;
        private bool _planked = false;

        public ProjectileMover(IProjectileMoverWindUpper projectileMoverWindUpper, IProjectileMoverPlanker projectileMoverPlanker,
            IProjectileRotator projectileRotator)
        {
            _projectileMoverWindUpper = projectileMoverWindUpper;
            _projectileMoverPlanker = projectileMoverPlanker;
            _projectileRotator = projectileRotator;
        }

        public IProjectileMover Create()
        {
            return new ProjectileMover(_projectileMoverWindUpper.Create(), _projectileMoverPlanker.Create(), _projectileRotator);
        }

        public void Move(Projectile projectile)
        {
            var frameTime = Raylib.GetFrameTime();

            if (projectile.Modifiers.WindUpDuration > 0)
                _projectileMoverWindUpper.WindUp(projectile);
            else if (projectile.Modifiers.PlankDuration > 0)
                _projectileMoverPlanker.Plank(projectile);

            var currentFrameSpeed = projectile.Modifiers.SpeedMultiplier * projectile.Speed;

            //projectile.Modifiers.VerticalPositionIncrement += ((projectile.Modifiers.VerticalPositionMultiplier * projectile.Modifiers.VerticalPositionIncrement) - projectile.Modifiers.VerticalPositionIncrement) * frameTime;
            projectile.Modifiers.VerticalPositionOffset += projectile.Modifiers.VerticalPositionIncrement * frameTime;

            projectile.Move(currentFrameSpeed.X * frameTime, currentFrameSpeed.Y * frameTime + projectile.Modifiers.VerticalPositionOffset*frameTime);
        }
    }
}
