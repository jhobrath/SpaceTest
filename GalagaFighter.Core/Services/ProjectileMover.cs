using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
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
        private bool _planked = false;

        public ProjectileMover(IProjectileMoverWindUpper projectileMoverWindUpper, IProjectileMoverPlanker projectileMoverPlanker)
        {
            _projectileMoverWindUpper = projectileMoverWindUpper;
            _projectileMoverPlanker = projectileMoverPlanker;
        }

        public IProjectileMover Create()
        {
            return new ProjectileMover(_projectileMoverWindUpper.Create(), _projectileMoverPlanker.Create());
        }

        public void Move(Projectile projectile)
        {
            if (projectile.Modifiers.WindUpDuration > 0)
                _projectileMoverWindUpper.WindUp(projectile);
            else if (projectile.Modifiers.PlankDuration > 0)
                _projectileMoverPlanker.Plank(projectile);

            var frameTime = Raylib.GetFrameTime();
            projectile.Move(projectile.Speed.X * frameTime, projectile.Speed.Y * frameTime);
        }
    }
}
